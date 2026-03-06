using MediatR;
using System;

namespace TellMe.Application.Features.Role
{
    public class CreateRoleCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
