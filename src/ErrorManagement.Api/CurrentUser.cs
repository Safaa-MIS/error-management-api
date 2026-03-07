using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ErrorManagement.Shared.Interfaces;

namespace ErrorManagement.Api;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _http;
    public CurrentUser(IHttpContextAccessor http) => _http = http;

    private ClaimsPrincipal? User => _http.HttpContext?.User;

    public Guid UserId => Guid.TryParse(User?.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id) ? id : Guid.Empty;
    public string Username => User?.FindFirstValue(JwtRegisteredClaimNames.UniqueName) ?? string.Empty;
    public string DisplayName => User?.FindFirstValue("displayName") ?? string.Empty;
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public bool HasPermission(string permission)
        => User?.Claims.Any(c =>
               c.Type == "permission" &&
               string.Equals(c.Value, permission, StringComparison.OrdinalIgnoreCase)) ?? false;

    public IEnumerable<string> GetPermissions()
        => User?.Claims
               .Where(c => c.Type == "permission")
               .Select(c => c.Value)
           ?? [];
}