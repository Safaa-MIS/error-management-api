namespace ErrorManagement.PatientServices.Application.Interfaces;

public sealed record PatientServiceResultDto(
    string RequestNo,
    string ServiceType,
    string Eid,
    string Mrn,
    string PatientName,
    string Status,
    string CreatedAt);