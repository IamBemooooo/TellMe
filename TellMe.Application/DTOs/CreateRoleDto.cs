using System;

namespace TellMe.Application.DTOs
{
    public class CreateRoleDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool isActive { get; set; }
        public List<Guid> PermissionIds { get; set; } = new();
    }
}
