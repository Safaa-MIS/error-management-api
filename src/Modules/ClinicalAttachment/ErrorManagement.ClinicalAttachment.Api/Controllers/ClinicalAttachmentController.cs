using ErrorManagement.ClinicalAttachment.Application.CheckFacilitiesSpeciality;
using ErrorManagement.ClinicalAttachment.Application.CheckUsersExists;
using ErrorManagement.ClinicalAttachment.Application.GetIcpUserByEid;
using ErrorManagement.ClinicalAttachment.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErrorManagement.ClinicalAttachment.Api;

[ApiController]
[Route("api/clinical-attachment")]
[Authorize]
public sealed class ClinicalAttachmentController : ControllerBase
{
    private readonly ISender _mediator;
    public ClinicalAttachmentController(ISender mediator) => _mediator = mediator;

    [HttpGet("icp-user")]
    [Authorize(Policy = ClinicalAttachmentPermissions.IcpView)]
    public async Task<IActionResult> GetIcpUser([FromQuery] string eid, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(eid))
            return BadRequest(new { message = "EID is required." });

        var result = await _mediator.Send(new GetIcpUserByEidQuery(eid), ct);
        return result is null ? NotFound(new { message = "User not found." }) : Ok(result);
    }

    [HttpGet("users-exists")]
    [Authorize(Policy = ClinicalAttachmentPermissions.UsersExistsView)]
    public async Task<IActionResult> CheckUsersExists(
        [FromQuery] string field, [FromQuery] string value, CancellationToken ct)
        => Ok(await _mediator.Send(new CheckUsersExistsQuery(field, value), ct));

    [HttpGet("facilities-speciality")]
    [Authorize(Policy = ClinicalAttachmentPermissions.FacilitiesSpecialityView)]
    public async Task<IActionResult> CheckFacilitiesSpeciality(
        [FromQuery] string field, [FromQuery] string value, CancellationToken ct)
        => Ok(await _mediator.Send(new CheckFacilitiesSpecialityQuery(field, value), ct));
}