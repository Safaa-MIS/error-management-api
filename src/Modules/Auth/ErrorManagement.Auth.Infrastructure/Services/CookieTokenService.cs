
using ErrorManagement.Auth.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ErrorManagement.Auth.Infrastructure.Services;

public sealed class CookieTokenService : ICookieTokenService
{
    private readonly CookieAuthOptions _opts;

    public CookieTokenService(IOptions<CookieAuthOptions> opts)
    {
        _opts = opts.Value;
    }

    public void SetTokens(HttpResponse response, string accessToken, string refreshToken, int expiresInSeconds)
    {
        var sameSite = _opts.SameSiteStrict ? SameSiteMode.Strict : SameSiteMode.Lax;

        response.Cookies.Append(_opts.AccessTokenName, accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = _opts.SecureOnly,
            SameSite = sameSite,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds)
        });

        //response.Cookies.Append(_opts.RefreshTokenName, refreshToken, new CookieOptions
        //{
        //    HttpOnly = true,
        //    Secure = _opts.SecureOnly,
        //    SameSite = sameSite,
        //    Path = "/api/auth/refresh",
        //    Expires = DateTimeOffset.UtcNow.AddDays(_jwt.RefreshTokenExpiresDays)
        //});
    }
    public void SetTokens(HttpResponse response, string accessToken, int expiresInSeconds)
    {
        var sameSite = _opts.SameSiteStrict ? SameSiteMode.Strict : SameSiteMode.Lax;

        response.Cookies.Append(_opts.AccessTokenName, accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = _opts.SecureOnly,
            SameSite = sameSite,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds)
        });

        //response.Cookies.Append(_opts.RefreshTokenName, refreshToken, new CookieOptions
        //{
        //    HttpOnly = true,
        //    Secure = _opts.SecureOnly,
        //    SameSite = sameSite,
        //    Path = "/api/auth/refresh",
        //    Expires = DateTimeOffset.UtcNow.AddDays(_jwt.RefreshTokenExpiresDays)
        //});
    }

    public void ClearTokens(HttpResponse response)
    {
        var sameSite = _opts.SameSiteStrict ? SameSiteMode.Strict : SameSiteMode.Lax;
        var expired = DateTimeOffset.UtcNow.AddDays(-1);

        response.Cookies.Append(_opts.AccessTokenName, string.Empty, new CookieOptions
        { HttpOnly = true, Secure = _opts.SecureOnly, SameSite = sameSite, Path = "/", Expires = expired });

        //response.Cookies.Append(_opts.RefreshTokenName, string.Empty, new CookieOptions
        //{ HttpOnly = true, Secure = _opts.SecureOnly, SameSite = sameSite, Path = "/api/auth/refresh", Expires = expired });
    }

    public string? GetAccessToken(HttpRequest request) => request.Cookies[_opts.AccessTokenName];
  //  public string? GetRefreshToken(HttpRequest request) => request.Cookies[_opts.RefreshTokenName];
}