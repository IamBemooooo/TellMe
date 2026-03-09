using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Application.DTOs;

namespace TellMe.Application.Features.Permission.Commands
{
    public class AssignPermissionsToRoleCommand : IRequest<Unit>
    {
        public AssignPermissionsToRoleDto Dto { get; set; } = null!;
    }
}
