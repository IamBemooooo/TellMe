using MediatR;
using TellMe.Application.DTOs;
using TellMe.Core.Common;

namespace TellMe.Application.Features.Role.Commands
{
    public class UpdateRoleCommand : IRequest<Result<RoleDto>>
    {
        public UpdateRoleDto RoleDto { get; set; } = null!;
    }
}