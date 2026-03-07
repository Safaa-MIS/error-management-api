// Lives in Application so Auth.Api can inject it without referencing Infrastructure
using Microsoft.AspNetCore.Http;

namespace ErrorManagement.Auth.Application.Interfaces;

public interface ICookieTokenService
{
    void SetTokens(HttpResponse response, string accessToken, int expiresInSeconds);
 //   void SetTokens(HttpResponse response, string accessToken, string refreshToken, int expiresInSeconds);
    void ClearTokens(HttpResponse response);
    string? GetAccessToken(HttpRequest request);
  //  string? GetRefreshToken(HttpRequest request);
}