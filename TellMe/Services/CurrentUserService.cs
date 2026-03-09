using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TellMe.Application.Common.Interfaces;
using TellMe.Data;

namespace TellMe.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, AppDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

        public Guid? GetUserId()
        {
            var id = Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(id, out var g) ? g : null;
        }

        public string? GetUsername() => Principal?.FindFirst(ClaimTypes.Name)?.Value;

        public IReadOnlyCollection<string> GetPermissions()
        {
            if (Principal == null)
                return Array.Empty<string>();

            var perms = Principal.FindAll("permission")
                .Select(c => c.Value?.Trim())
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return perms;
        }

        public async Task<string?> GetFullNameAsync(CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null) return null;
            return user.Name;
        }

        public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;
    }
}
