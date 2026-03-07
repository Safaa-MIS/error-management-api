namespace ErrorManagement.ClinicalAttachment.Application.Interfaces;

public interface IClinicalAttachmentApiClient
{
    Task<ClinicalUserDto?> GetIcpUserByEidAsync(string eid, CancellationToken ct = default);
    Task<IReadOnlyList<ClinicalUserDto>> CheckUsersExistsAsync(string field, string value, CancellationToken ct = default);
    Task<IReadOnlyList<ClinicalUserDto>> CheckFacilitiesSpecialityAsync(string field, string value, CancellationToken ct = default);
}