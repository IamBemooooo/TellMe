using TellMe.Core.Entities;
using System.Collections.Generic;

namespace TellMe.Infrastructure.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user, List<string> perrmissions);
        bool ValidateToken(string token);
    }
}
