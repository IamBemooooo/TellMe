using MediatR;
using Microsoft.EntityFrameworkCore;
using TellMe.Core.Common;
using TellMe.Data;

namespace TellMe.Application.Features.Role.Commands
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result<bool>>
    {
        private readonly AppDbContext _context;

        public DeleteRoleCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteRoleCommand request, CancellationToken ct)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == request.Id, ct);

            if (role == null)
                return Result<bool>.Failure("Không tìm thấy vai trò để xóa.");

            role.IsDeleted = true; 
            await _context.SaveChangesAsync(ct);

            return Result<bool>.Success(true);
        }
    }
}