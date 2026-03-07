using ErrorManagement.ClinicalAttachment.Application.Interfaces;
using MediatR;

namespace ErrorManagement.ClinicalAttachment.Application.CheckFacilitiesSpeciality;

public sealed class CheckFacilitiesSpecialityHandler
    : IRequestHandler<CheckFacilitiesSpecialityQuery, IReadOnlyList<ClinicalUserDto>>
{
    private readonly IClinicalAttachmentApiClient _client;
    public CheckFacilitiesSpecialityHandler(IClinicalAttachmentApiClient client) => _client = client;

    public Task<IReadOnlyList<ClinicalUserDto>> Handle(
        CheckFacilitiesSpecialityQuery request, CancellationToken ct)
        => _client.CheckFacilitiesSpecialityAsync(request.Field, request.Value.Trim(), ct);
}