using ErrorManagement.Auth.Application.Interfaces;
using ErrorManagement.Auth.Application.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ErrorManagement.Auth.Api;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ICookieTokenService _cookies;

    public AuthController(ISender mediator, ICookieTokenService cookies)
    {
        _mediator = mediator;
        _cookies = cookies;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        _cookies.SetTokens(Response, result.AccessToken,result.ExpiresInSeconds);
        return Ok(new
        {
            displayName = result.DisplayName,
            permissions = result.Permissions,
            accessTokenExpiresAt = DateTime.UtcNow.AddSeconds(result.ExpiresInSeconds).ToString("o"),
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        _cookies.ClearTokens(Response);
        return NoContent();
    }
}