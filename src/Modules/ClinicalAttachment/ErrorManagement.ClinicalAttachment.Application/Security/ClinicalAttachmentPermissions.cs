namespace ErrorManagement.ClinicalAttachment.Application.Security;

public static class ClinicalAttachmentPermissions
{
    public const string View = "clinicalattachment.view";
    public const string HistoryView = "clinicalattachment.history.view";
    public const string IcpView = "clinicalattachment.icp.view";
    public const string UsersExistsView = "clinicalattachment.users-exists.view";
    public const string FacilitiesSpecialityView = "clinicalattachment.facilities-speciality.view";

    public static IReadOnlyCollection<string> All { get; } =
    [
        View,
        HistoryView,
        IcpView,
        UsersExistsView,
        FacilitiesSpecialityView
    ];
}