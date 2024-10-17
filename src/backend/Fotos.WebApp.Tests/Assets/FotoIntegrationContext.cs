using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Fotos.WebApp.Features.Photos;
using Fotos.WebApp.Features.Photos.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fotos.WebApp.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed class FotoIntegrationContext
{
    internal AddPhotoToMainStorage AddPhotoToMainStorage => _host.Services.GetRequiredService<AddPhotoToMainStorage>();
    internal GetOriginalUri GetOriginalUri => _host.Services.GetRequiredService<GetOriginalUri>();
    internal GetThumbnailUri GetThumbnailUri => _host.Services.GetRequiredService<GetThumbnailUri>();
    internal ReadOriginalPhoto ReadOriginalPhoto => _host.Services.GetRequiredService<ReadOriginalPhoto>();
    internal CreateThumbnail CreateThumbnail => _host.Services.GetRequiredService<CreateThumbnail>();
    internal AddPhotoToThumbnailStorage AddPhotoToThumbnailStorage => _host.Services.GetRequiredService<AddPhotoToThumbnailStorage>();
    internal RemovePhotoOriginal RemovePhotoOriginal => _host.Services.GetRequiredService<RemovePhotoOriginal>();
    internal RemovePhotoThumbnail RemovePhotoThumbnail => _host.Services.GetRequiredService<RemovePhotoThumbnail>();

    internal BlobContainerClient PhotosContainer
    {
        get
        {
            var storage = _host.Services.GetRequiredService<BlobServiceClient>();
            var containerName = _host.Services.GetRequiredService<IConfiguration>()["MainStorage:PhotosContainer"];

            return storage.GetBlobContainerClient(containerName);
        }
    }

    internal OnNewPhotoUploaded OnNewPhotoUploaded => _host.Services.GetRequiredService<OnNewPhotoUploaded>();
    internal OnPhotoRemoved OnPhotoRemoved => _host.Services.GetRequiredService<OnPhotoRemoved>();
    internal ServiceBusClient ServiceBusClient => _host.Services.GetRequiredService<ServiceBusClient>();
    internal string MainTopicName => _host.Services.GetRequiredService<IConfiguration>()["ServiceBus:MainTopic"]!;

    private readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureServices(ConfigureServices)
        .ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MainStorage:blobServiceUri"] = "UseDevelopmentStorage=true",
                ["MainStorage:PhotosContainer"] = "fotostests",
                ["ServiceBus:fullyQualifiedNamespace"] = "arkiabus.servicebus.windows.net",
                ["ServiceBus:MainTopic"] = "tests-fotos-main"
            });
        }).ConfigureLogging(builder => builder
            .AddFilter("Azure.Identity", LogLevel.Warning)
            .AddFilter("Azure.Core", LogLevel.Warning)
            .AddFilter("Azure.Messaging.ServiceBus", LogLevel.Warning)
        )
        .Build();

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddFotosAzureStorage(context.Configuration);
        services.AddFotosServiceBus(context.Configuration);
        services.AddFotosImageProcessing();
    }
}
