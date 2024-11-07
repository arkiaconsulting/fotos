﻿using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Fotos.App;
using Fotos.App.Api.Account;
using Fotos.App.Api.Adapters;
using Fotos.App.Api.PhotoAlbums;
using Fotos.App.Api.Photos;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fotos.Tests.Backend.Assets;

public sealed class FotoIntegrationContext
{
    internal AddPhotoToMainStorage AddPhotoToMainStorage => _host.Services.GetRequiredService<AddPhotoToMainStorage>();
    internal GetOriginalStorageUri GetOriginalUri => _host.Services.GetRequiredService<GetOriginalStorageUri>();
    internal GetThumbnailStorageUri GetThumbnailUri => _host.Services.GetRequiredService<GetThumbnailStorageUri>();
    internal ReadOriginalPhotoFromStorage ReadOriginalPhoto => _host.Services.GetRequiredService<ReadOriginalPhotoFromStorage>();
    internal CreateThumbnail CreateThumbnail => _host.Services.GetRequiredService<CreateThumbnail>();
    internal AddPhotoToThumbnailStorage AddPhotoToThumbnailStorage => _host.Services.GetRequiredService<AddPhotoToThumbnailStorage>();
    internal RemovePhotoOriginalFromStorage RemovePhotoOriginal => _host.Services.GetRequiredService<RemovePhotoOriginalFromStorage>();
    internal RemovePhotoThumbnailFromStorage RemovePhotoThumbnail => _host.Services.GetRequiredService<RemovePhotoThumbnailFromStorage>();
    internal ExtractExifMetadata ExtractExifMetadata => _host.Services.GetRequiredService<ExtractExifMetadata>();
    internal AddPhotoToStore StorePhotoData => _host.Services.GetRequiredService<AddPhotoToStore>();
    internal ListPhotosFromStore ListPhotos => _host.Services.GetRequiredService<ListPhotosFromStore>();
    internal RemovePhotoFromStore RemovePhotoData => _host.Services.GetRequiredService<RemovePhotoFromStore>();
    internal GetPhotoFromStore GetPhoto => _host.Services.GetRequiredService<GetPhotoFromStore>();
    internal AddSessionDataToStore StoreSessionData => _host.Services.GetRequiredService<AddSessionDataToStore>();
    internal GetSessionDataFromStore GetSessionData => _host.Services.GetRequiredService<GetSessionDataFromStore>();
    internal AddUserToStore AddUserToStore => _host.Services.GetRequiredService<AddUserToStore>();
    internal GetAlbumPhotoCountFromStore GetAlbumPhotoCount => _host.Services.GetRequiredService<GetAlbumPhotoCountFromStore>();
    internal Container PhotosData
    {
        get
        {
            var configuration = _host.Services.GetRequiredService<IConfiguration>();

            return _host.Services.GetRequiredService<CosmosClient>()
                .GetDatabase(configuration["CosmosDb:DatabaseId"])
                .GetContainer(configuration["CosmosDb:ContainerId"]);
        }
    }

    internal BlobContainerClient PhotosContainer
    {
        get
        {
            var storage = _host.Services.GetRequiredService<BlobServiceClient>();
            var containerName = _host.Services.GetRequiredService<IConfiguration>()["MainStorage:PhotosContainer"];

            return storage.GetBlobContainerClient(containerName);
        }
    }

    internal Container SessionData
    {
        get
        {
            var configuration = _host.Services.GetRequiredService<IConfiguration>();

            return _host.Services.GetRequiredService<CosmosClient>()
                .GetDatabase(configuration["CosmosDb:DatabaseId"])
                .GetContainer(configuration["CosmosDb:SessionDataContainerId"]);
        }
    }

    internal Container UsersData
    {
        get
        {
            var configuration = _host.Services.GetRequiredService<IConfiguration>();

            return _host.Services.GetRequiredService<CosmosClient>()
                .GetDatabase(configuration["CosmosDb:DatabaseId"])
                .GetContainer(configuration["CosmosDb:UsersContainerId"]);
        }
    }

    internal OnNewPhotoUploaded OnNewPhotoUploaded => _host.Services.GetRequiredService<OnNewPhotoUploaded>();
    internal OnPhotoRemoved OnPhotoRemoved => _host.Services.GetRequiredService<OnPhotoRemoved>();
    internal ServiceBusClient ServiceBusClient => _host.Services.GetRequiredService<ServiceBusClient>();
    internal string TestTopicName => _host.Services.GetRequiredService<IConfiguration>()["ServiceBus:MainTopic"]!;
    internal string ProduceThumbnailSubscriptionName => _host.Services.GetRequiredService<IConfiguration>()["ServiceBus:ProduceThumbnailSubscription"]!;
    internal string RemovePhotosBinariesSubscriptionName => _host.Services.GetRequiredService<IConfiguration>()["ServiceBus:RemovePhotoBinariesSubscription"]!;

    private readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureServices(ConfigureServices)
        .ConfigureAppConfiguration((_, config) =>
        {
            config.AddJsonFile("tests.settings.json", optional: true);
            config.AddEnvironmentVariables();
        }).ConfigureLogging(builder => builder
            .AddFilter("Azure.Identity", LogLevel.Warning)
            .AddFilter("Azure.Core", LogLevel.Warning)
            .AddFilter("Azure.Messaging.ServiceBus", LogLevel.Warning)
        )
        .Build();

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<TokenCredential>(_ => new DefaultAzureCredential());
        services.AddMemoryCache();
        services.AddSingleton<InstrumentationConfig>();

        services.AddFotosAzureStorage(context.Configuration);
        services.AddFotosServiceBus(context.Configuration);
        services.AddFotosImageProcessing();
        services.AddFotosCosmosDb(context.Configuration);
    }
}