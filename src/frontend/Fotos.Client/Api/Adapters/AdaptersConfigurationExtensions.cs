using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Fotos.Client.Api.PhotoAlbums;
using Fotos.Client.Api.PhotoFolders;
using Fotos.Client.Api.Photos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Azure;

namespace Fotos.Client.Api.Adapters;

internal static class AdaptersConfigurationExtensions
{
    public static IServiceCollection AddPhotosAdapters(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<TokenCredential>(_ => new DefaultAzureCredential());

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
        .AddScoped<GetOriginalStorageUri>(sp => sp.GetRequiredService<AzurePhotoStorage>().GetOriginalUri)
        .AddScoped<GetThumbnailStorageUri>(sp => sp.GetRequiredService<AzurePhotoStorage>().GetThumbnailUri)
        .AddScoped<ReadOriginalPhotoFromStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().ReadOriginalPhoto)
        .AddScoped<AddPhotoToThumbnailStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().AddPhotoToThumbnailStorage)
        .AddScoped<RemovePhotoOriginalFromStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().RemovePhotoOriginal)
        .AddScoped<RemovePhotoThumbnailFromStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().RemovePhotoThumbnail);

        services.AddAzureClients(builder =>
        {
            var serviceUriOrConnectionString = configuration[$"{Constants.BlobServiceClientName}:blobServiceUri"];

            if (Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out var serviceUri))
            {
                builder.AddBlobServiceClient(serviceUri).WithName(Constants.BlobServiceClientName)
                .WithCredential(sp => sp.GetRequiredService<TokenCredential>());
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
        .AddScoped<OnPhotoRemoved>(sp => sp.GetRequiredService<AzureServiceBus>().OnPhotoRemoved)
        .AddScoped<OnThumbnailReady>(sp => sp.GetRequiredService<AzureServiceBus>().OnThumbnailReady);

        services.AddAzureClients(builder =>
        {
            var fqdn = configuration[$"{Constants.ServiceBusClientName}:fullyQualifiedNamespace"];

            builder.AddServiceBusClientWithNamespace(fqdn)
            .WithName(Constants.ServiceBusClientName)
            .WithCredential(sp => sp.GetRequiredService<TokenCredential>());
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
        services.AddSingleton<ExifMetadataService>();
        services.AddScoped<ExtractExifMetadata>(sp => sp.GetRequiredService<ExifMetadataService>().Extract);

        return services;
    }

    public static IServiceCollection AddFotosCosmosDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AzureCosmosDb>()
        .AddScoped<AddPhotoToStore>(sp => sp.GetRequiredService<AzureCosmosDb>().SavePhoto)
        .AddScoped<ListPhotosFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().ListPhotos)
        .AddScoped<RemovePhotoFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().RemovePhoto)
        .AddScoped<GetPhotoFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().GetPhoto)
        .AddScoped<AddFolderToStore>(sp => sp.GetRequiredService<AzureCosmosDb>().StoreFolder)
        .AddScoped<GetFoldersFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().GetFolders)
        .AddScoped<GetFolderFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().GetFolder)
        .AddScoped<RemoveFolderFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().RemoveFolder)
        .AddScoped<UpdateFolderInStore>(sp => sp.GetRequiredService<AzureCosmosDb>().UpsertFolder)
        .AddScoped<GetFolderAlbumsFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().GetAlbums)
        .AddScoped<AddAlbumToStore>(sp => sp.GetRequiredService<AzureCosmosDb>().StoreAlbum)
        .AddScoped<GetAlbumFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().GetAlbum)
        .AddScoped<AddSessionDataToStore>(sp => sp.GetRequiredService<AzureCosmosDb>().StoreSessionData)
        .AddScoped<GetSessionDataFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().GetSessionData);

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
