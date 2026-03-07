
namespace ErrorManagement.PatientServices.Application.Interfaces;

public interface IPatientServicesApiClient
{
    Task<IReadOnlyList<PatientServiceResultDto>> SearchAsync(
        string serviceType, string field, string value, CancellationToken ct = default);
}