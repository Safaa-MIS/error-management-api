using ErrorManagement.ClinicalAttachment.Application.Interfaces;
using MediatR;

namespace ErrorManagement.ClinicalAttachment.Application.CheckFacilitiesSpeciality;

public sealed record CheckFacilitiesSpecialityQuery(string Field, string Value)
    : IRequest<IReadOnlyList<ClinicalUserDto>>;