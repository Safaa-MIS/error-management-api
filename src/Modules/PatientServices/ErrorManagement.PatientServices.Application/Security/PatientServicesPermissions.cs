namespace ErrorManagement.PatientServices.Application.Security;

public static class PatientServicesPermissions
{
    public const string View = "patientservices.view";
    public const string MedicalReportView = "medicalreport.view";
    public const string SickLeaveView = "sickleave.view";

    public static IReadOnlyCollection<string> All { get; } =
    [
        View,
        MedicalReportView,
        SickLeaveView
    ];
}