using ErrorManagement.ClinicalAttachment.Application.Interfaces;
using MediatR;

namespace ErrorManagement.ClinicalAttachment.Application.GetIcpUserByEid;

public sealed record GetIcpUserByEidQuery(string Eid) : IRequest<ClinicalUserDto?>;