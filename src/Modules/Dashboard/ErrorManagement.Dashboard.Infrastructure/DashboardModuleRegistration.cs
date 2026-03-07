
using ErrorManagement.Dashboard.Application.GetDashboardCards;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ErrorManagement.Dashboard.Infrastructure;

public static class DashboardModuleRegistration
{
    public static IServiceCollection AddDashboardModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(GetDashboardCardsHandler).Assembly));
        return services;
    }
}