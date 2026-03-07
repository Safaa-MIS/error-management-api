using ErrorManagement.PatientServices.Application.Interfaces;
using MediatR;

namespace ErrorManagement.PatientServices.Application.MedicalReport.GetMedicalReport;

public sealed record GetMedicalReportQuery(string Field, string Value)
    : IRequest<IReadOnlyList<PatientServiceResultDto>>;