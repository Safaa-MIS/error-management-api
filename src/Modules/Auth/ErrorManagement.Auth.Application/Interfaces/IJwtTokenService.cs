// Auth.Api can inject it without referencing Infrastructure
namespace ErrorManagement.Auth.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(IAuthUser user);
   // string GenerateRefreshToken();
    int AccessTokenExpiresInSeconds { get; }
}