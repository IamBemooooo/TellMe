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

namespace TellMe.Application.Features.User.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler(
            AppDbContext context,
            IMapper mapper,
            ILogger<UpdateUserCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dto = request.UserDto;

                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

                if (user == null)
                    return Result<UserDto>.Failure($"Người dùng với Id {request.Id} không tồn tại");

                //var changedValues = new Dictionary<string, object?>();

                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result<UserDto>.Failure("Họ và tên là bắt buộc");

                if (user.Name != dto.Name)
                {
                    user.Name = dto.Name;
                    //changedValues["Name"] = dto.Name;
                }

                if (!string.IsNullOrWhiteSpace(dto.Email) && user.Email != dto.Email)
                {
                    user.Email = dto.Email;
                    //changedValues["Email"] = dto.Email;
                }

                if (dto.IsActive.HasValue && user.IsActive != dto.IsActive.Value)
                {
                    user.IsActive = dto.IsActive.Value;
                    //changedValues["IsActive"] = dto.IsActive.Value;
                }

                if (!string.IsNullOrWhiteSpace(dto.ProfileImageUrl) && user.ProfileImageUrl != dto.ProfileImageUrl)
                {
                    user.ProfileImageUrl = dto.ProfileImageUrl;
                    //changedValues["IsActive"] = dto.IsActive.Value;
                }

                if (dto.RoleId == Guid.Empty)
                    return Result<UserDto>.Failure("Phải chọn role");

                var role = await _context.Roles
                    .Include(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(r => r.Id == dto.RoleId && r.IsActive, cancellationToken);

                if (role == null)
                    return Result<UserDto>.Failure("Role không hợp lệ");

                var currentRoleId = user.UserRoles.FirstOrDefault()?.RoleId;

                if (currentRoleId != dto.RoleId)
                {
                    // Replace ExecuteDeleteAsync with removing tracked UserRole entities for compatibility
                    var userRolesToRemove = user.UserRoles.ToList();
                    if (userRolesToRemove.Any())
                    {
                        _context.UserRoles.RemoveRange(userRolesToRemove);
                    }

                    user.UserRoles.Clear();

                    user.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = dto.RoleId
                    });

                    //changedValues["RoleId"] = dto.RoleId;
                }

                await _context.SaveChangesAsync(cancellationToken);

                var updatedUser = await _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .FirstAsync(u => u.Id == user.Id, cancellationToken);

                var resultDto = _mapper.Map<UserDto>(updatedUser);

                return Result<UserDto>.SuccessResult(resultDto, "Cập nhật tài khoản thành công");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex,
                    "UpdateUser concurrency error. UserId={UserId}",
                    request.Id);

                return Result<UserDto>.Failure(
                    "Dữ liệu đã bị thay đổi bởi người khác, vui lòng tải lại và thử lại");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "UpdateUser crashed. UserId={UserId}",
                    request.Id);

                return Result<UserDto>.Failure("Cập nhật người dùng thất bại: " + ex.Message);
            }
        }
    }
}
