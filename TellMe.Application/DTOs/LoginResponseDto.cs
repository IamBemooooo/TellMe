using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiresAt { get; set; }
        public UserLoginDto User { get; set; } = null!;
        public List<string>? Permissions { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
