
namespace ErrorManagement.Dashboard.Application.GetDashboardCards;

public sealed record DashboardCardResponse(
    string Key,
    string Title,
    string Description,
    string BaseRoute,
    string Permission,
    string? Icon,
    string? Badge,
    IReadOnlyList<DashboardCardChildLinkResponse>? Children,
    int SortOrder);

public sealed record DashboardCardChildLinkResponse(string Title, string Route, string Permission);