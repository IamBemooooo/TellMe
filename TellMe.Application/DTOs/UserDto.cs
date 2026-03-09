using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Application.DTOs
{
    public class UserLoginDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserBriefDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // Profile
        public List<RoleBriefDto> Roles { get; set; } = new();
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // Profile
        public string? ProfileImageUrl { get; set; }

        public string? Address { get; set; }

        // Role
        public List<RoleDto> Roles { get; set; } = new();
    }

    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public Guid RoleId { get; set; }
        public string? ProfileImageUrl { get; set; }
    }

    public class UpdateUserDto
    {
        public string? Email { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public Guid RoleId { get; set; } = new();
        public string? ProfileImageUrl { get; set; }
    }
}
