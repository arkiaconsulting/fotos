using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Fotos.WebApp.Adapters;
using Fotos.WebApp.Features.Photos;
using Microsoft.Azure.Cosmos;
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
    internal ExtractExifMetadata ExtractExifMetadata => _host.Services.GetRequiredService<ExtractExifMetadata>();
    internal StorePhotoData StorePhotoData => _host.Services.GetRequiredService<StorePhotoData>();
    internal ListPhotos ListPhotos => _host.Services.GetRequiredService<ListPhotos>();
    internal RemovePhotoData RemovePhotoData => _host.Services.GetRequiredService<RemovePhotoData>();
    internal GetPhoto GetPhoto => _host.Services.GetRequiredService<GetPhoto>();
    internal Container PhotosData => _host.Services.GetRequiredService<CosmosClient>().GetDatabase("fotos").GetContainer("photos-test");

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
                ["ServiceBus:MainTopic"] = "tests-fotos-main",
                ["CosmosDb:AccountEndpoint"] = "https://localhost:8081",
                ["CosmosDb:AccountKey"] = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                ["CosmosDb:DatabaseId"] = "fotos",
                ["CosmosDb:ContainerId"] = "photos-test",
                ["CosmosDb:FoldersContainerId"] = "folders-test",
                ["CosmosDb:AlbumsContainerId"] = "albums-test",
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
        services.AddFotosCosmosDb(context.Configuration);
    }
}
