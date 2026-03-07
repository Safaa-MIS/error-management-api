using ErrorManagement.PatientServices.Application.Interfaces;
using ErrorManagement.PatientServices.Application.SearchPatientServices;
using ErrorManagement.PatientServices.Infrastructure.ExternalApis;
using ErrorManagement.Shared.Http;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.Timeout;

namespace ErrorManagement.PatientServices.Infrastructure;

public static class PatientServicesModuleRegistration
{
    public static IServiceCollection AddPatientServicesModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IPatientServicesApiClient, PatientServicesApiClient>(client =>
        {
            client.BaseAddress = new Uri(
                configuration["ExternalApis:PatientServices:BaseUrl"]
                ?? throw new InvalidOperationException(
                    "ExternalApis:PatientServices:BaseUrl is not configured."));
            client.Timeout = TimeSpan.FromSeconds(30);
        }).AddInternalApiResilience("clinical-attachment");


        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(SearchPatientServicesHandler).Assembly));

        return services;
    }
}