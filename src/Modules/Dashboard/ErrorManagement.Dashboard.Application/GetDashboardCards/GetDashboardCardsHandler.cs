
using ErrorManagement.Shared.Interfaces;
using MediatR;

namespace ErrorManagement.Dashboard.Application.GetDashboardCards;

public sealed class GetDashboardCardsHandler
    : IRequestHandler<GetDashboardCardsQuery, IReadOnlyList<DashboardCardResponse>>
{
    private readonly ICurrentUser _currentUser;

    private static readonly DashboardCardResponse[] AllCards =
    [
        new(Key: "clinical-attachment",
            Title: "Clinical Attachment",
            Description: "Investigate clinical attachment requests and user statuses",
            BaseRoute: "/applications/clinical-attachment",
            Permission: "clinicalattachment.view",
            Icon: "bi-clipboard2-pulse",
            Badge: null, Children: null, SortOrder: 1),

        new(Key: "patient-services",
            Title: "Patient Services",
            Description: "Search sick leave, medical reports and patient records",
            BaseRoute: "/applications/patient-services",
            Permission: "patientservices.view",
            Icon: "bi-person-heart",
            Badge: null, Children: null, SortOrder: 2),
    ];

    public GetDashboardCardsHandler(ICurrentUser currentUser) => _currentUser = currentUser;

    public Task<IReadOnlyList<DashboardCardResponse>> Handle(
        GetDashboardCardsQuery request, CancellationToken ct)
    {
        var permissions = _currentUser.GetPermissions()
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        IReadOnlyList<DashboardCardResponse> result = AllCards
            .Where(c => permissions.Contains(c.Permission))
            .OrderBy(c => c.SortOrder)
            .ToList();

        return Task.FromResult(result);
    }
}