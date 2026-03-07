using ErrorManagement.PatientServices.Application.Interfaces;
using MediatR;

namespace ErrorManagement.PatientServices.Application.MedicalReport.GetMedicalReport;

public sealed class GetMedicalReportHandler
    : IRequestHandler<GetMedicalReportQuery, IReadOnlyList<PatientServiceResultDto>>
{
    private readonly IPatientServicesApiClient _client;
    public GetMedicalReportHandler(IPatientServicesApiClient client) => _client = client;

    public Task<IReadOnlyList<PatientServiceResultDto>> Handle(
        GetMedicalReportQuery request, CancellationToken ct)
        => _client.SearchAsync("Medical Report", request.Field, request.Value.Trim(), ct);
}