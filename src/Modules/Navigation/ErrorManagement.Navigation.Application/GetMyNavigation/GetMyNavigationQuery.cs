
using MediatR;

namespace ErrorManagement.Navigation.Application.GetMyNavigation;

public sealed record GetMyNavigationQuery : IRequest<NavigationResponse>;