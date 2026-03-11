using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Application.Common.Interfaces;
using TellMe.Application.DTOs;
using TellMe.Core.Common;
using TellMe.Core.Entities;
using TellMe.Data;
using TellMe.Infrastructure.Services;

namespace TellMe.Application.Features.User.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public CreateUserCommandHandler(
            AppDbContext context,
            IMapper mapper,
            IPasswordHasher passwordHasher,
            ILogger<CreateUserCommandHandler> logger,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var dto = request.UserDto;

                // ===== BASIC VALIDATION =====
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result<UserDto>.Failure("Họ và tên là bắt buộc");

                if (string.IsNullOrWhiteSpace(dto.Username))
                    return Result<UserDto>.Failure("Tên đăng nhập là bắt buộc");

                if (string.IsNullOrWhiteSpace(dto.Password))
                    return Result<UserDto>.Failure("Mật khẩu là bắt buộc");

                if (dto.Password.Length < 8)
                    return Result<UserDto>.Failure("Mật khẩu phải có ít nhất 8 ký tự");

                if (dto.Password != dto.ConfirmPassword)
                    return Result<UserDto>.Failure("Mật khẩu xác nhận không khớp");

                var exists = await _context.Users
                    .Where(u => !u.IsDeleted)
                    .AnyAsync(u =>
                        u.Username == dto.Username, cancellationToken);

                if (exists)
                    return Result<UserDto>.Failure("Tên đăng nhập đã tồn tại");

                // ===== ROLES =====
                if (dto.RoleId == Guid.Empty)
                    return Result<UserDto>.Failure("Phải chọn role");

                var role = await _context.Roles
                    .Include(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(r => r.Id == dto.RoleId && r.IsActive, cancellationToken);

                if (role == null)
                    return Result<UserDto>.Failure("Role không hợp lệ");

                // ===== CREATE USER =====
                var user = new TellMe.Core.Entities.User
                {
                    Name = dto.Name,
                    Username = dto.Username,
                    Email = dto.Email,
                    IsActive = dto.IsActive,
                    //IsZaloUser = false,
                    PasswordHash = _passwordHasher.HashPassword(dto.Password),
                    ProfileImageUrl = dto.ProfileImageUrl
                };


                // ===== USER ROLES =====
                user.UserRoles.Add(new UserRole
                {
                    RoleId = dto.RoleId
                });

                _context.Users.Add(user);
                await _context.SaveChangesAsync(cancellationToken);

                var userWithIncludes = await _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .FirstAsync(u => u.Id == user.Id, cancellationToken);

                var resultDto = _mapper.Map<UserDto>(userWithIncludes);
                await transaction.CommitAsync(cancellationToken);

                return Result<UserDto>.SuccessResult(resultDto, "Thêm mới tài khoản thành công");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex,
                    "CreateUser crashed. Username={Username}",
                    request.UserDto?.Username);
                return Result<UserDto>.Failure("Thêm mới tài khoản thất bại: " + ex.Message);
            }
        }
    }
}
