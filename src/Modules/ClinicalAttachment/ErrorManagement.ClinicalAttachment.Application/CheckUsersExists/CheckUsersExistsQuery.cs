using ErrorManagement.ClinicalAttachment.Application.Interfaces;
using MediatR;

namespace ErrorManagement.ClinicalAttachment.Application.CheckUsersExists;

public sealed record CheckUsersExistsQuery(string Field, string Value)
    : IRequest<IReadOnlyList<ClinicalUserDto>>;