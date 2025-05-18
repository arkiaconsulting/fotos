using Azure.Messaging.ServiceBus;
using Fotos.App.Application.Photos;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;

namespace Fotos.App.Adapters.Messaging;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AzureServiceBus>()
        .AddScoped<OnNewPhotoUploaded>(sp => sp.GetRequiredService<AzureServiceBus>().OnNewPhotoUploaded)
        .AddScoped<OnPhotoRemoved>(sp => sp.GetRequiredService<AzureServiceBus>().OnPhotoRemoved);

        services.AddOptions<ServiceBusOptions>()
            .BindConfiguration(ServiceBusOptions.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.Configure<ServiceBusSenderOptions>(_ => { });
        services.Configure<ServiceBusClientOptions>(options =>
        {
            options.RetryOptions.MaxRetries = 5;
            options.RetryOptions.Mode = ServiceBusRetryMode.Exponential; // default
        });

        services.AddAzureClients(builder =>
        {
            builder.AddClient<ServiceBusSender, ServiceBusClientOptions>(
                (clientOptions, credential, provider) =>
                {
                    var options = provider.GetRequiredService<IOptions<ServiceBusOptions>>().Value;
                    var senderOptions = provider.GetRequiredService<IOptions<ServiceBusSenderOptions>>().Value;

                    return new ServiceBusClient(options.FullyQualifiedNamespace, credential, clientOptions)
                    .CreateSender(options.MainTopic, senderOptions);
                });
        });

        return services;
    }
}
