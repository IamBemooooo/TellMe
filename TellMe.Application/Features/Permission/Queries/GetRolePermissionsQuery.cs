using MediatR;
using TellMe.Application.DTOs;

namespace TellMe.Application.Features.Permission.Queries
{
    public class GetRolePermissionsQuery : IRequest<List<PermissionDto>>
    {
        public Guid RoleId { get; set; }
    }
}
