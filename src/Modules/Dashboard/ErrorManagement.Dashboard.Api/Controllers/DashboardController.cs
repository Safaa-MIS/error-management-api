using ErrorManagement.Dashboard.Application.GetDashboardCards;
using ErrorManagement.Dashboard.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErrorManagement.Dashboard.Api;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public sealed class DashboardController : ControllerBase
{
    private readonly ISender _mediator;
    public DashboardController(ISender mediator) => _mediator = mediator;

    [HttpGet("dashboard-cards")]
    [Authorize(Policy = DashboardPermissions.View)]
    public async Task<IActionResult> GetDashboardCards(CancellationToken ct)
        => Ok(await _mediator.Send(new GetDashboardCardsQuery(), ct));
}