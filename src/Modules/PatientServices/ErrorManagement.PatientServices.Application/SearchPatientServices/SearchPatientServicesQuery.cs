using ErrorManagement.PatientServices.Application.Interfaces;
using MediatR;

namespace ErrorManagement.PatientServices.Application.SearchPatientServices;

public sealed record SearchPatientServicesQuery(string Category, string Field, string Value)
    : IRequest<IReadOnlyList<PatientServiceResultDto>>;