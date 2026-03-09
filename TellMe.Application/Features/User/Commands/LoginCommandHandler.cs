using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TellMe.Application.Common.Behaviors;
using TellMe.Application.DTOs;
using TellMe.Application.Features.Permission.Queries;
using TellMe.Core.Common;
using TellMe.Core.Entities;
using TellMe.Data;
using TellMe.Infrastructure.Services;

namespace TellMe.Application.Features.User.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
    {
        private readonly AppDbContext _context;
        private readonly IMediator _mediator;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;

        public LoginCommandHandler(
            AppDbContext context,
            IMediator mediator,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            IMapper mapper)
        {
            _context = context;
            _mediator = mediator;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
        }

        public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.IsActive && !u.IsDeleted && u.Username == request.LoginDto.Username, cancellationToken);

            if (user == null)
            {
                return Result<LoginResponseDto>.Failure("Tên người dùng hoặc mật khẩu không hợp lệ");
            }

            // Check if account is locked
            if (user.IsDeleted)
                return Result<LoginResponseDto>.Failure("Tài khoản đã bị xóa");

            if (!user.IsActive)
                return Result<LoginResponseDto>.Failure("Tài khoản đã bị vô hiệu hóa");

            // Verify password
            if (!_passwordHasher.VerifyPassword(request.LoginDto.Password, user.PasswordHash))
            {
                return Result<LoginResponseDto>.Failure("Tên người dùng hoặc mật khẩu không hợp lệ");
            }

            var roleIds = await _context.UserRoles
               .Where(ur => ur.UserId == user.Id)
               .Select(ur => ur.RoleId)
               .ToListAsync(cancellationToken);

            var permissionDtos = new List<PermissionDto>();
            foreach (var rid in roleIds)
            {
                var query = new GetRolePermissionsQuery { RoleId = rid };
                var perms = await _mediator.Send(query, cancellationToken);
                if (perms != null) permissionDtos.AddRange(perms);
            }

            var permissionNames = permissionDtos
                .Select(p => p.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct()
                .ToList();

            // Generate refresh token prior to executing retriable transaction so we can return it unchanged
            var refreshToken = JwtTokenHelper.GenerateRefreshToken();
            var refreshTokenHash = JwtTokenHelper.HashRefreshToken(refreshToken);
            var refreshTokenExpiresAt = DateTime.UtcNow.AddHours(24);

            // Use execution strategy to execute transaction as a retriable unit (required for MySQL retrying strategy)
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);

                // Revoke existing sessions by loading and updating them (avoids ExecuteUpdateAsync compatibility issues)
                var sessionsToRevoke = await _context.UserSessions
                    .Where(x => x.UserId == user.Id && !x.IsRevoked)
                    .ToListAsync(cancellationToken);

                if (sessionsToRevoke.Any())
                {
                    foreach (var s in sessionsToRevoke)
                    {
                        s.IsRevoked = true;
                    }

                    _context.UserSessions.UpdateRange(sessionsToRevoke);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                // Create new session with the pre-generated token hash
                _context.UserSessions.Add(new UserSessions
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = refreshTokenHash,
                    CreatedAt = DateTime.UtcNow,
                    ExpiredAt = refreshTokenExpiresAt,
                    IsRevoked = false
                });

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            });

            var accessToken = _jwtTokenService.GenerateToken(user, permissionNames);

            var responselogin = new LoginResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshTokenExpiresAt,
                User = _mapper.Map<UserLoginDto>(user),
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                Permissions = permissionNames,
            };

            return Result<LoginResponseDto>.SuccessResult(responselogin, "Đăng nhập thành công");
        }
    }
}
