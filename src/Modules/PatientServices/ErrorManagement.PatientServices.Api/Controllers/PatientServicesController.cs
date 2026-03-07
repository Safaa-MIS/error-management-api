using ErrorManagement.PatientServices.Application.MedicalReport.GetMedicalReport;
using ErrorManagement.PatientServices.Application.SearchPatientServices;
using ErrorManagement.PatientServices.Application.Security;
using ErrorManagement.PatientServices.Application.SickLeave.GetSickLeave;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErrorManagement.PatientServices.Api;

[ApiController]
[Route("api/patient-services")]
[Authorize]
public sealed class PatientServicesController : ControllerBase
{
    private readonly ISender _mediator;
    public PatientServicesController(ISender mediator) => _mediator = mediator;

    [HttpGet("search")]
    [Authorize(Policy = PatientServicesPermissions.View)]
    public async Task<IActionResult> Search(
        [FromQuery] string category,
        [FromQuery] string field,
        [FromQuery] string value,
        CancellationToken ct)
        => Ok(await _mediator.Send(new SearchPatientServicesQuery(category, field, value), ct));

    [HttpGet("medical-report")]
    [Authorize(Policy = PatientServicesPermissions.MedicalReportView)]
    public async Task<IActionResult> GetMedicalReport(
        [FromQuery] string field,
        [FromQuery] string value,
        CancellationToken ct)
        => Ok(await _mediator.Send(new GetMedicalReportQuery(field, value), ct));

    [HttpGet("sick-leave")]
    [Authorize(Policy = PatientServicesPermissions.SickLeaveView)]
    public async Task<IActionResult> GetSickLeave(
        [FromQuery] string field,
        [FromQuery] string value,
        CancellationToken ct)
        => Ok(await _mediator.Send(new GetSickLeaveQuery(field, value), ct));
}