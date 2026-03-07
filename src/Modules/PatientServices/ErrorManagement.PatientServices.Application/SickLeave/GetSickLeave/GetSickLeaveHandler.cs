using ErrorManagement.PatientServices.Application.Interfaces;
using ErrorManagement.PatientServices.Application.SickLeave.GetSickLeave;
using MediatR;

namespace ErrorManagement.PatientServices.Application.GetSickLeave;

public sealed class GetSickLeaveHandler
    : IRequestHandler<GetSickLeaveQuery, IReadOnlyList<PatientServiceResultDto>>
{
    private readonly IPatientServicesApiClient _client;
    public GetSickLeaveHandler(IPatientServicesApiClient client) => _client = client;

    public Task<IReadOnlyList<PatientServiceResultDto>> Handle(
        GetSickLeaveQuery request, CancellationToken ct)
        => _client.SearchAsync("Sick Leave", request.Field, request.Value.Trim(), ct);
}