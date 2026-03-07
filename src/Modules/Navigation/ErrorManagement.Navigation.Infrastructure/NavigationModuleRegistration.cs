using ErrorManagement.Navigation.Application.GetMyNavigation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory; 

namespace ErrorManagement.Navigation.Infrastructure;

public static class NavigationModuleRegistration
{
    public static IServiceCollection AddNavigationModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(GetMyNavigationHandler).Assembly));
        return services;
    }
}