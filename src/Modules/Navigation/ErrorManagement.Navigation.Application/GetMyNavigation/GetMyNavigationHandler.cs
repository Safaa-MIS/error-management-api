
using ErrorManagement.Shared.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace ErrorManagement.Navigation.Application.GetMyNavigation;

public sealed class GetMyNavigationHandler : IRequestHandler<GetMyNavigationQuery, NavigationResponse>
{
    private readonly ICurrentUser _currentUser;
    private readonly IMemoryCache _cache;

    // All possible modules — filtered per user by permission at request time
    private static readonly NavModuleDto[] AllModules =
    [
        new(Key: "clinical-attachment",
            Title: "Clinical Attachment",
            Description: "Manage and investigate clinical attachment requests",
            BaseRoute: "/applications/clinical-attachment",
            Permission: "clinicalattachment.view",
            Icon: "bi-clipboard2-pulse",
            SortOrder: 1,
            Tools:
            [
                new("Attachment History",      "/applications/clinical-attachment",                              "clinicalattachment.history.view"),
                new("Check ICP User Data",     "/applications/clinical-attachment/check-user-icp-data",         "clinicalattachment.icp.view"),
                new("Check Users Exists",      "/applications/clinical-attachment/check-users-exists",          "clinicalattachment.users-exists.view"),
                new("Facilities & Speciality", "/applications/clinical-attachment/check-facilities-speciality", "clinicalattachment.facilities-speciality.view"),
            ]),

        new(Key: "patient-services",
            Title: "Patient Services",
            Description: "Search and manage patient medical services",
            BaseRoute: "/applications/patient-services",
            Permission: "patientservices.view",
            Icon: "bi-person-heart",
            SortOrder: 2,
            Tools:
            [
                new("Search",         "/applications/patient-services",                "patientservices.view"),
                new("Medical Report", "/applications/patient-services/medical-report", "medicalreport.view"),
                new("Sick Leave",     "/applications/patient-services/sick-leave",     "sickleave.view"),
            ]),
    ];

    public GetMyNavigationHandler(ICurrentUser currentUser, IMemoryCache cache)
    {
        _currentUser = currentUser;
        _cache = cache;
    }

    public Task<NavigationResponse> Handle(GetMyNavigationQuery request, CancellationToken ct)
    {
        // Cache per user — navigation is static per permission set
        var cacheKey = $"nav_{_currentUser.UserId}";

        var response = _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

            // HashSet for O(1) permission lookup
            var permissions = _currentUser.GetPermissions()
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var modules = AllModules
                .Where(m => permissions.Contains(m.Permission))
                .Select(m => m with
                {
                    Tools = m.Tools
                        .Where(t => permissions.Contains(t.Permission))
                        .ToList()
                })
                .OrderBy(m => m.SortOrder)
                .ToList();

            return new NavigationResponse(
                DisplayName: _currentUser.DisplayName,
                Modules: modules,
                Permissions: [.. permissions]);
        });

        return Task.FromResult(response!);
    }
}