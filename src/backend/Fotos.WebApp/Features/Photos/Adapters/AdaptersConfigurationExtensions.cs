using Azure.Core;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Azure;

namespace Fotos.WebApp.Features.Photos.Adapters;

internal static class AdaptersConfigurationExtensions
{
    public static IServiceCollection AddPhotosAdapters(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFotosAzureStorage(configuration);
        services.AddFotosServiceBus(configuration);
        services.AddFotosImageProcessing();
        services.AddFotosCosmosDb(configuration);

        return services;
    }

    public static IServiceCollection AddFotosAzureStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AzurePhotoStorage>()
        .AddScoped<AddPhotoToMainStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().AddOriginalPhoto)
        .AddScoped<GetOriginalUri>(sp => sp.GetRequiredService<AzurePhotoStorage>().GetOriginalUri)
        .AddScoped<GetThumbnailUri>(sp => sp.GetRequiredService<AzurePhotoStorage>().GetThumbnailUri)
        .AddScoped<ReadOriginalPhoto>(sp => sp.GetRequiredService<AzurePhotoStorage>().ReadOriginalPhoto)
        .AddScoped<AddPhotoToThumbnailStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().AddPhotoToThumbnailStorage)
        .AddScoped<RemovePhotoOriginal>(sp => sp.GetRequiredService<AzurePhotoStorage>().RemovePhotoOriginal)
        .AddScoped<RemovePhotoThumbnail>(sp => sp.GetRequiredService<AzurePhotoStorage>().RemovePhotoThumbnail);

        services.AddAzureClients(builder =>
        {
            var serviceUriOrConnectionString = configuration[$"{Constants.BlobServiceClientName}:blobServiceUri"];

            if (Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out var serviceUri))
            {
                builder.AddBlobServiceClient(serviceUri).WithName(Constants.BlobServiceClientName);
            }
            else
            {
                builder.AddBlobServiceClient(serviceUriOrConnectionString).WithName(Constants.BlobServiceClientName);
            }
        });

        services.AddSingleton(sp =>
        {
            var factory = sp.GetRequiredService<IAzureClientFactory<BlobServiceClient>>();

            return factory.CreateClient(Constants.BlobServiceClientName);
        });

        return services;
    }

    public static IServiceCollection AddFotosServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AzureServiceBus>()
        .AddScoped<OnNewPhotoUploaded>(sp => sp.GetRequiredService<AzureServiceBus>().OnNewPhotoUploaded)
        .AddScoped<OnPhotoRemoved>(sp => sp.GetRequiredService<AzureServiceBus>().OnPhotoRemoved);

        services.AddAzureClients(builder =>
        {
            var fqdn = configuration[$"{Constants.ServiceBusClientName}:fullyQualifiedNamespace"];

            builder.AddServiceBusClientWithNamespace(fqdn).WithName(Constants.ServiceBusClientName);
        });
        services.AddSingleton(sp =>
        {
            var factory = sp.GetRequiredService<IAzureClientFactory<ServiceBusClient>>();

            return factory.CreateClient(Constants.ServiceBusClientName);
        });
        return services;
    }

    public static IServiceCollection AddFotosImageProcessing(this IServiceCollection services)
    {
        services.AddScoped<CreateThumbnail>(_ => SkiaSharpImageProcessing.CreateThumbnail);
        services.AddScoped<ExtractExifMetadata>(_ => ExifMetadataService.Extract);

        return services;
    }

    public static IServiceCollection AddFotosCosmosDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AzureCosmosDb>()
        .AddScoped<StorePhotoData>(sp => sp.GetRequiredService<AzureCosmosDb>().SavePhoto)
        .AddScoped<ListPhotos>(sp => sp.GetRequiredService<AzureCosmosDb>().ListPhotos)
        .AddScoped<RemovePhotoData>(sp => sp.GetRequiredService<AzureCosmosDb>().RemovePhoto)
        .AddScoped<GetPhoto>(sp => sp.GetRequiredService<AzureCosmosDb>().GetPhoto);

        services.AddSingleton(sp =>
        {
            var endpoint = configuration["CosmosDb:AccountEndpoint"];
            var key = configuration["CosmosDb:AccountKey"];

            if (string.IsNullOrWhiteSpace(key))
            {
                var credential = sp.GetRequiredService<TokenCredential>();
                var clientBuilder = new CosmosClientBuilder(endpoint, credential)
                .WithSerializerOptions(new() { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase });

                return clientBuilder.Build();
            }
            else
            {
                var clientBuilder = new CosmosClientBuilder(endpoint, key)
                .WithSerializerOptions(new() { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase });

                return clientBuilder.Build();
            }
        });

        return services;
    }
}
