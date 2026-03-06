using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Application.Common.Behaviors;
using TellMe.Application.DTOs;
using TellMe.Core.Common;
using TellMe.Core.Entities;
using TellMe.Data;
using TellMe.Infrastructure.Services;
using TellMe.Application.Features.Permission.Queries;

namespace TellMe.Application.Features.User.Commands
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponseDto>>
    {
        private readonly AppDbContext _context;
        private readonly IMediator _mediator;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;

        public RefreshTokenCommandHandler(AppDbContext context, IMediator mediator, IJwtTokenService jwtTokenService, IMapper mapper)
        {
            _context = context;
            _mediator = mediator;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
        }

        public async Task<Result<LoginResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshTokenHash = JwtTokenHelper.HashRefreshToken(request.RefreshToken);
            var tokenEntity = await _context.UserSessions
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.Token == refreshTokenHash &&
                    !x.IsRevoked &&
                    x.ExpiredAt > DateTime.UtcNow,
                    cancellationToken);

            if (tokenEntity == null)
                return Result<LoginResponseDto>.Failure("Refresh token không hợp lệ");

            var user = tokenEntity.User;

            if (user == null || user.IsDeleted || !user.IsActive || user.IsLocked)
            {
                tokenEntity.IsRevoked = true;
                await _context.SaveChangesAsync(cancellationToken);
                return Result<LoginResponseDto>.Failure("Tài khoản không hợp lệ");
            }

            var permissionDtos = await _mediator.Send(
                new GetUserPermissionsQuery { UserId = user.Id },
                cancellationToken);

            var permissionNames = permissionDtos
                .Select(p => p.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct()
                .ToList();

            var newAccessToken = _jwtTokenService.GenerateToken(user, permissionNames);
            var newRefreshToken = JwtTokenHelper.GenerateRefreshToken();
            var newRefreshTokenHash = JwtTokenHelper.HashRefreshToken(newRefreshToken);

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var refreshTokenExpiresAt = DateTime.UtcNow.AddHours(24);
            tokenEntity.IsRevoked = true;
            _context.UserSessions.Update(tokenEntity);

            _context.UserSessions.Add(new UserSessions
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newRefreshTokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiredAt = refreshTokenExpiresAt,
                IsRevoked = false
            });

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<LoginResponseDto>.SuccessResult(new LoginResponseDto
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiresAt = refreshTokenExpiresAt,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = _mapper.Map<UserLoginDto>(user),
                Permissions = permissionNames
            });
        }
    }
}
