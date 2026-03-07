using ErrorManagement.PatientServices.Application.Interfaces;
using MediatR;

namespace ErrorManagement.PatientServices.Application.SearchPatientServices;

public sealed class SearchPatientServicesHandler
    : IRequestHandler<SearchPatientServicesQuery, IReadOnlyList<PatientServiceResultDto>>
{
    private readonly IPatientServicesApiClient _client;
    public SearchPatientServicesHandler(IPatientServicesApiClient client) => _client = client;

    public Task<IReadOnlyList<PatientServiceResultDto>> Handle(
        SearchPatientServicesQuery request, CancellationToken ct)
    {
        var serviceType = request.Category switch
        {
            "medical-report" => "Medical Report",
            "sick-leave" => "Sick Leave",
            _ => "Patient Services"
        };
        return _client.SearchAsync(serviceType, request.Field, request.Value.Trim(), ct);
    }
}