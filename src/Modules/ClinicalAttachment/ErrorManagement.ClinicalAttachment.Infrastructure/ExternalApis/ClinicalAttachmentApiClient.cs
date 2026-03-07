using System.Net.Http.Json;
using ErrorManagement.ClinicalAttachment.Application.Interfaces;


namespace ErrorManagement.ClinicalAttachment.Infrastructure.ExternalApis;

public sealed class ClinicalAttachmentApiClient : IClinicalAttachmentApiClient
{
    private readonly HttpClient _http;
    public ClinicalAttachmentApiClient(HttpClient http) => _http = http;

    public async Task<ClinicalUserDto?> GetIcpUserByEidAsync(string eid, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"icp-user?eid={Uri.EscapeDataString(eid)}", ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ClinicalUserDto>(cancellationToken: ct);
    }

    public async Task<IReadOnlyList<ClinicalUserDto>> CheckUsersExistsAsync(
        string field, string value, CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<ClinicalUserDto>>(
            $"users-exists?field={Uri.EscapeDataString(field)}&value={Uri.EscapeDataString(value)}", ct);
        return result ?? [];
    }

    public async Task<IReadOnlyList<ClinicalUserDto>> CheckFacilitiesSpecialityAsync(
        string field, string value, CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<ClinicalUserDto>>(
            $"facilities-speciality?field={Uri.EscapeDataString(field)}&value={Uri.EscapeDataString(value)}", ct);
        return result ?? [];
    }
}