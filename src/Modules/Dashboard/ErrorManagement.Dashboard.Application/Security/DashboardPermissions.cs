namespace ErrorManagement.Dashboard.Application.Security;

public static class DashboardPermissions
{
    public const string View = "dashboard.view";

    public static IReadOnlyCollection<string> All { get; } =
    [
        View
    ];
}