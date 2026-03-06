using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Application.Common.Exceptions;
using TellMe.Application.DTOs;
using TellMe.Data;

namespace TellMe.Application.Features.Permission.Queries
{
    public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, List<PermissionDto>>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GetRolePermissionsQueryHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PermissionDto>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
        {
            // Check if role exists
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == request.RoleId, cancellationToken);
            if (!roleExists)
            {
                throw new NotFoundException(nameof(Core.Entities.Role), request.RoleId);
            }

            var permissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == request.RoleId)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<PermissionDto>>(permissions);
        }
    }
}
