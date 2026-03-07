using ErrorManagement.PatientServices.Application.Interfaces;
using MediatR;

namespace ErrorManagement.PatientServices.Application.SickLeave.GetSickLeave;

public sealed record GetSickLeaveQuery(string Field, string Value)
    : IRequest<IReadOnlyList<PatientServiceResultDto>>;