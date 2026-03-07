using ErrorManagement.ClinicalAttachment.Application.Interfaces;
using MediatR;

namespace ErrorManagement.ClinicalAttachment.Application.GetIcpUserByEid;

public sealed class GetIcpUserByEidHandler : IRequestHandler<GetIcpUserByEidQuery, ClinicalUserDto?>
{
    private readonly IClinicalAttachmentApiClient _client;
    public GetIcpUserByEidHandler(IClinicalAttachmentApiClient client) => _client = client;

    public Task<ClinicalUserDto?> Handle(GetIcpUserByEidQuery request, CancellationToken ct)
        => _client.GetIcpUserByEidAsync(request.Eid.Trim(), ct);
}