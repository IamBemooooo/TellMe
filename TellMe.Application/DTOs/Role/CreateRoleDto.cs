using System;

namespace TellMe.Application.DTOs.Role
{
    public class CreateRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
