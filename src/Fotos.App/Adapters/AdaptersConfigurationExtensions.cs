using Azure.Core;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Fotos.App.Application.Folders;
using Fotos.App.Application.Photos;
using Fotos.App.Application.User;
using Fotos.App.Domain;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Azure;

namespace Fotos.App.Adapters;

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
        .AddScoped<GetOriginalStorageUri>(sp => sp.GetRequiredService<AzurePhotoStorage>().GetOriginalUri)
        .AddScoped<GetThumbnailStorageUri>(sp => sp.GetRequiredService<AzurePhotoStorage>().GetThumbnailUri)
        .AddScoped<ReadOriginalPhotoFromStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().ReadOriginalPhoto)
        .AddScoped<AddPhotoToThumbnailStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().AddPhotoToThumbnailStorage)
        .AddScoped<RemovePhotoOriginalFromStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().RemovePhotoOriginal)
        .AddScoped<RemovePhotoThumbnailFromStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().RemovePhotoThumbnail);

        services.AddAzureClients(builder =>
        {
            var serviceUriOrConnectionString = configuration[$"MainStorage:blobServiceUri"]
            ?? throw new InvalidOperationException("MainStorage:blobServiceUri is not configured.");

            builder.AddBlobServiceClient(new Uri(serviceUriOrConnectionString))
            .WithCredential(sp => sp.GetRequiredService<TokenCredential>());

            builder.AddClient<BlobContainerClient, BlobClientOptions>(
                (_, _, sp) =>
                {
                    var containerName = sp.GetRequiredService<IConfiguration>()["MainStorage:PhotosContainer"];

                    return sp.GetRequiredService<BlobServiceClient>().GetBlobContainerClient(containerName);

                }).WithName(Constants.PhotosBlobContainer);
        });

        services.AddSingleton<SasUriGenerator>();

        return services;
    }

    public static IServiceCollection AddFotosServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AzureServiceBus>()
        .AddScoped<OnNewPhotoUploaded>(sp => sp.GetRequiredService<AzureServiceBus>().OnNewPhotoUploaded)
        .AddScoped<OnPhotoRemoved>(sp => sp.GetRequiredService<AzureServiceBus>().OnPhotoRemoved);

        services.AddAzureClients(builder =>
        {
            var fqdn = configuration["ServiceBus:fullyQualifiedNamespace"];

            builder.AddServiceBusClientWithNamespace(fqdn)
            .WithCredential(sp => sp.GetRequiredService<TokenCredential>());

            builder.AddClient<ServiceBusSender, ServiceBusSenderOptions>(
                (_, _, provider) =>
                {
                    return provider.GetRequiredService<ServiceBusClient>()
                    .CreateSender(configuration["ServiceBus:MainTopic"]);
                });
        });

        return services;
    }

    public static IServiceCollection AddFotosImageProcessing(this IServiceCollection services)
    {
        services.AddSingleton<SkiaSharpImageProcessing>();
        services.AddScoped<CreateThumbnail>(sp => sp.GetRequiredService<SkiaSharpImageProcessing>().CreateThumbnail);
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
        .AddScoped<GetSessionDataFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().GetSessionData)
        .AddScoped<FindUserInStore>(sp => sp.GetRequiredService<AzureCosmosDb>().FindUser)
        .AddScoped<AddUserToStore>(sp => sp.GetRequiredService<AzureCosmosDb>().StoreUser)
        .AddScoped<GetAlbumPhotoCountFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().GetAlbumPhotoCount);

        services.AddSingleton(sp =>
        {
            var endpoint = configuration["Cosmos:Endpoint"];
            var key = configuration["Cosmos:AccountKey"];
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
