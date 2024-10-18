using Azure.Core;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Fotos.WebApp.Features.PhotoAlbums;
using Fotos.WebApp.Features.PhotoFolders;
using Fotos.WebApp.Features.Photos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Azure;

namespace Fotos.WebApp.Adapters;

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
        .AddScoped<GetPhoto>(sp => sp.GetRequiredService<AzureCosmosDb>().GetPhoto)
        .AddScoped<StoreNewFolder>(sp => sp.GetRequiredService<AzureCosmosDb>().StoreFolder)
        .AddScoped<GetFolders>(sp => sp.GetRequiredService<AzureCosmosDb>().GetFolders)
        .AddScoped<GetFolder>(sp => sp.GetRequiredService<AzureCosmosDb>().GetFolder)
        .AddScoped<RemoveFolder>(sp => sp.GetRequiredService<AzureCosmosDb>().RemoveFolder)
        .AddScoped<GetFolderAlbums>(sp => sp.GetRequiredService<AzureCosmosDb>().GetAlbums)
        .AddScoped<AddAlbum>(sp => sp.GetRequiredService<AzureCosmosDb>().StoreAlbum)
        .AddScoped<GetAlbum>(sp => sp.GetRequiredService<AzureCosmosDb>().GetAlbum);

        services.AddSingleton(sp =>
        {
            var endpoint = configuration["CosmosDb:AccountEndpoint"];
            var key = configuration["CosmosDb:AccountKey"];
            var clientBuilder = string.IsNullOrWhiteSpace(key)
                ? new CosmosClientBuilder(endpoint, sp.GetRequiredService<TokenCredential>())
                : new CosmosClientBuilder(endpoint, key);

            return clientBuilder
            .WithContentResponseOnWrite(false)
            .WithConnectionModeDirect()
            .WithLimitToEndpoint(true)
            .WithSystemTextJsonSerializerOptions(new(System.Text.Json.JsonSerializerDefaults.Web))
            .Build();
        });

        return services;
    }
}
