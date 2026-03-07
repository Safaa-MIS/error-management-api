using ErrorManagement.ClinicalAttachment.Application.GetIcpUserByEid;
using ErrorManagement.ClinicalAttachment.Application.Interfaces;
using ErrorManagement.ClinicalAttachment.Infrastructure.ExternalApis;
using ErrorManagement.Shared.Http;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.Timeout;

namespace ErrorManagement.ClinicalAttachment.Infrastructure;

public static class ClinicalAttachmentModuleRegistration
{
    public static IServiceCollection AddClinicalAttachmentModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IClinicalAttachmentApiClient, ClinicalAttachmentApiClient>(client =>
        {
            client.BaseAddress = new Uri(
                configuration["ExternalApis:ClinicalAttachment:BaseUrl"]
                ?? throw new InvalidOperationException(
                    "ExternalApis:ClinicalAttachment:BaseUrl is not configured."));
        })
          .AddInternalApiResilience("clinical-attachment");

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(GetIcpUserByEidHandler).Assembly));

        return services;
    }
}