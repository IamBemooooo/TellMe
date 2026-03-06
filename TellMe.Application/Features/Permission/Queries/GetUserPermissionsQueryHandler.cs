using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TellMe.Application.Common.Exceptions;
using TellMe.Application.DTOs;
using TellMe.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TellMe.Application.Features.Permission.Queries
{
    public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, List<PermissionDto>>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GetUserPermissionsQueryHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PermissionDto>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
        {
            // Check if user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
            if (!userExists)
            {
                throw new NotFoundException(nameof(Core.Entities.User), request.UserId);
            }

            // Get role ids for the user
            var roleIds = await _context.UserRoles
                .Where(ur => ur.UserId == request.UserId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);

            if (roleIds == null || !roleIds.Any())
            {
                return new List<PermissionDto>();
            }

            // Get permission ids for those roles
            var permissionIds = await _context.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => rp.PermissionId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (permissionIds == null || !permissionIds.Any())
            {
                return new List<PermissionDto>();
            }

            // Get active permissions
            var permissions = await _context.Permissions
                .Where(p => permissionIds.Contains(p.Id) && p.IsActive)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<PermissionDto>>(permissions);
        }
    }
}
