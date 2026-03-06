using System.Security.Cryptography;
using System.Text;

namespace TellMe.Application.Common.Behaviors
{
    public static class JwtTokenHelper
    {
        public static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public static string HashRefreshToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

    }
}
