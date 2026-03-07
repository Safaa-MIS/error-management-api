
using ErrorManagement.Auth.Application.Behaviours;
using ErrorManagement.Auth.Application.Interfaces;
using ErrorManagement.Auth.Application.Login;
using ErrorManagement.Auth.Infrastructure.Persistence;
using ErrorManagement.Auth.Infrastructure.Repositories;
using ErrorManagement.Auth.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ErrorManagement.Auth.Infrastructure;

public static class AuthModuleRegistration
{
    public static IServiceCollection AddAuthModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Section));
        services.Configure<CookieAuthOptions>(configuration.GetSection(CookieAuthOptions.Section));

        // DB-first: no MigrationsAssembly — schema already exists
        services.AddDbContext<AuthDbContext>(opts =>
            opts.UseSqlServer(
                configuration.GetConnectionString("ErrorManagementDb"),
                sql => sql.CommandTimeout(30)));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICookieTokenService, CookieTokenService>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(LoginHandler).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        });

        services.AddValidatorsFromAssembly(typeof(LoginValidator).Assembly);

        return services;
    }
}