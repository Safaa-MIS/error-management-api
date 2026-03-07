
namespace ErrorManagement.Navigation.Application.GetMyNavigation;

public sealed record NavigationResponse(
    string DisplayName,
    IReadOnlyList<NavModuleDto> Modules,
    IReadOnlyList<string> Permissions);

public sealed record NavModuleDto(
    string Key,
    string Title,
    string? Description,
    string BaseRoute,
    string Permission,
    string? Icon,
    int SortOrder,
    IReadOnlyList<NavToolDto> Tools);

public sealed record NavToolDto(string Title, string Route, string Permission);