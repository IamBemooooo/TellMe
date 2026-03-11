using MediatR;
using Microsoft.EntityFrameworkCore;
using TellMe.Application.Features.Role.Commands;
using TellMe.Core.Common;
using TellMe.Data;

namespace TellMe.Application.Features.Role.Handlers
{    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result<bool>>
    {
        private readonly AppDbContext _context;

        public DeleteRoleCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteRoleCommand request, CancellationToken ct)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, ct);

            if (role == null)
            {
                return Result<bool>.Failure("Không tìm thấy vai trò để xóa hoặc vai trò đã bị xóa trước đó.");
            }

            if (role.IsActive)
            {
                return Result<bool>.Failure("Vai trò đang Active, không thể xóa.");
            }
            var hasUserAssigned = await _context.UserRoles
                .AnyAsync(ur => ur.RoleId == request.Id, ct);

            if (hasUserAssigned)
            {
                return Result<bool>.Failure("Không thể xóa vì vẫn còn người dùng đang giữ vai trò này.");
            }

            role.IsDeleted = true;
            await _context.SaveChangesAsync(ct);

            return Result<bool>.Success(true);
        }
    }
}