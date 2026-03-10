using MediatR;
using System;
using TellMe.Application.DTOs;
using TellMe.Core.Common;

namespace TellMe.Application.Features.Role.Commands
{
    public class CreateRoleCommand : IRequest<Result<RoleDto>>
    {
        public CreateRoleDto RoleDto { get; set; } = null!;


    }
}
