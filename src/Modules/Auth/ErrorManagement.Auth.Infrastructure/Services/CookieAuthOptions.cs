
namespace ErrorManagement.Auth.Infrastructure.Services;

public sealed class CookieAuthOptions
{
    public const string Section = "CookieAuth";
    public string AccessTokenName { get; init; } = "em_at";
  //  public string RefreshTokenName { get; init; } = "em_rt";
    public bool SecureOnly { get; init; } = true;
    public bool SameSiteStrict { get; init; } = true;
}