using MediatR;
using Microsoft.EntityFrameworkCore;
using TellMe.Application.Common.Exceptions;
using TellMe.Data;

namespace TellMe.Application.Features.Permission.Commands
{
    public class AssignPermissionsToRoleCommandHandler : IRequestHandler<AssignPermissionsToRoleCommand, Unit>
    {
        private readonly AppDbContext _context;

        public AssignPermissionsToRoleCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AssignPermissionsToRoleCommand request, CancellationToken cancellationToken)
        {
            // Check if role exists
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == request.Dto.RoleId, cancellationToken);
            if (!roleExists)
            {
                throw new NotFoundException(nameof(Core.Entities.Role), request.Dto.RoleId);
            }

            // Check if all permissions exist
            var permissionsExist = await _context.Permissions
                .Where(p => request.Dto.PermissionIds.Contains(p.Id))
                .CountAsync(cancellationToken);

            if (permissionsExist != request.Dto.PermissionIds.Count)
            {
                throw new InvalidOperationException("One or more permissions not found");
            }

            // Get existing role permissions
            var existingRolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == request.Dto.RoleId && request.Dto.PermissionIds.Contains(rp.PermissionId))
                .Select(rp => rp.PermissionId)
                .ToListAsync(cancellationToken);

            // Filter out already assigned permissions
            var permissionsToAssign = request.Dto.PermissionIds
                .Where(permissionId => !existingRolePermissions.Contains(permissionId))
                .ToList();

            if (permissionsToAssign.Count == 0)
            {
                throw new InvalidOperationException("All specified permissions are already assigned to this role");
            }

            // Assign new permissions
            var rolePermissions = permissionsToAssign.Select(permissionId => new Core.Entities.RolePermission
            {
                RoleId = request.Dto.RoleId,
                PermissionId = permissionId
            }).ToList();

            _context.RolePermissions.AddRange(rolePermissions);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
