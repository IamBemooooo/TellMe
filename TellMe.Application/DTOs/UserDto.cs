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
}
