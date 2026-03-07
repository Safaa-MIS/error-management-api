
using ErrorManagement.Navigation.Application.GetMyNavigation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErrorManagement.Navigation.Api;

[ApiController]
[Route("api/navigation")]
[Authorize]
public sealed class NavigationController : ControllerBase
{
    private readonly ISender _mediator;
    public NavigationController(ISender mediator) => _mediator = mediator;

    /// <summary>GET /api/navigation/mylist — drives Angular sidebar and permission set</summary>
    [HttpGet("mylist")]
    public async Task<IActionResult> GetMyNavigation(CancellationToken ct)
        => Ok(await _mediator.Send(new GetMyNavigationQuery(), ct));
}