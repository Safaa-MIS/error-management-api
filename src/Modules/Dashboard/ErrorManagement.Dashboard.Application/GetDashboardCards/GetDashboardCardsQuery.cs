
using MediatR;

namespace ErrorManagement.Dashboard.Application.GetDashboardCards;

public sealed record GetDashboardCardsQuery : IRequest<IReadOnlyList<DashboardCardResponse>>;