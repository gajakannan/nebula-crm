using System.Security.Claims;
using Nebula.Application.Common;

namespace Nebula.Api.Services;

public class HttpCurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal User => httpContextAccessor.HttpContext?.User
        ?? throw new InvalidOperationException("No HttpContext available.");

    public string Subject => User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
    public string? DisplayName => User.FindFirstValue("name") ?? User.FindFirstValue(ClaimTypes.Name);
    public IReadOnlyList<string> Roles => User.FindAll("role").Concat(User.FindAll(ClaimTypes.Role)).Select(c => c.Value).Distinct().ToList();
    public IReadOnlyList<string> Regions => User.FindAll("regions").Select(c => c.Value).ToList();
}
