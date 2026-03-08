
namespace ErrorManagement.Auth.Infrastructure.Services;

public sealed class CookieAuthOptions
{
    public const string Section = "CookieAuth";
    public string AccessTokenName { get; init; } = "em_at";
    public bool SecureOnly { get; init; } = true;
    public bool SameSiteStrict { get; init; } = true;
}