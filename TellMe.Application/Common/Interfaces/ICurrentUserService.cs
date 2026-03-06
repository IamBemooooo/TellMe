using System.Security.Claims;

namespace TellMe.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        ClaimsPrincipal? Principal { get; }
        Guid? GetUserId();
        string? GetUsername();
        IReadOnlyCollection<string> GetPermissions();
        bool IsAuthenticated { get; }
        Task<string?> GetFullNameAsync(CancellationToken cancellationToken = default);
        Task<string?> GetZaloIdAsync(CancellationToken cancellationToken = default);
        Guid? ServiceCounterId { get; }
        string GetCounterCode();
    }
}
