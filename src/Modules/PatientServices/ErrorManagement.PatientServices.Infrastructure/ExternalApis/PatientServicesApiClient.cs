using System.Net.Http.Json;
using ErrorManagement.PatientServices.Application.Interfaces;

namespace ErrorManagement.PatientServices.Infrastructure.ExternalApis;

public sealed class PatientServicesApiClient : IPatientServicesApiClient
{
    private readonly HttpClient _http;
    public PatientServicesApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<PatientServiceResultDto>> SearchAsync(
        string serviceType, string field, string value, CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<PatientServiceResultDto>>(
            $"search?serviceType={Uri.EscapeDataString(serviceType)}" +
            $"&field={Uri.EscapeDataString(field)}" +
            $"&value={Uri.EscapeDataString(value)}", ct);
        return result ?? [];
    }
}