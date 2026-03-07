using ErrorManagement.ClinicalAttachment.Application.Interfaces;
using MediatR;

namespace ErrorManagement.ClinicalAttachment.Application.CheckUsersExists;

public sealed class CheckUsersExistsHandler
    : IRequestHandler<CheckUsersExistsQuery, IReadOnlyList<ClinicalUserDto>>
{
    private readonly IClinicalAttachmentApiClient _client;
    public CheckUsersExistsHandler(IClinicalAttachmentApiClient client) => _client = client;

    public Task<IReadOnlyList<ClinicalUserDto>> Handle(
        CheckUsersExistsQuery request, CancellationToken ct)
        => _client.CheckUsersExistsAsync(request.Field, request.Value.Trim(), ct);
}