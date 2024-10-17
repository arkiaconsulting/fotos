using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Fotos.WebApp.Types;
using Microsoft.Extensions.Azure;

namespace Fotos.WebApp.Features.Photos.Adapters;

internal static class AdaptersConfigurationExtensions
{
    public static IServiceCollection AddPhotosAdapters(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSingleton<List<PhotoEntity>>(_ => [])
            .AddScoped<StorePhotoData>(sp => photo =>
            {
                var store = sp.GetRequiredService<List<PhotoEntity>>();
                store.Add(photo);

                return Task.CompletedTask;
            })
            .AddScoped<ListPhotos>(sp =>
            {
                var store = sp.GetRequiredService<List<PhotoEntity>>();

                return (albumId) =>
                {
                    var photos = store.Where(x => x.Id.AlbumId == albumId.Id).ToList();

                    return Task.FromResult<IReadOnlyCollection<PhotoEntity>>(photos);
                };
            })
            .AddScoped<RemovePhotoData>(sp => (photoId) =>
            {
                var store = sp.GetRequiredService<List<PhotoEntity>>();

                store.RemoveAll(photo => photo.Id.Id == photoId.Id);

                return Task.CompletedTask;
            })
            .AddScoped<ExtractExifMetadata>((_) => (_) => Task.FromResult(new ExifMetadata(DateTime.Now)))
            .AddScoped<GetPhoto>(_ => (photoId) =>
            {
                var store = _.GetRequiredService<List<PhotoEntity>>();

                return Task.FromResult(store.Single(x => x.Id.Id == photoId.Id));
            });

        services.AddFotosAzureStorage(configuration);
        services.AddFotosServiceBus(configuration);
        services.AddFotosImageProcessing();

        return services;
    }

    public static IServiceCollection AddFotosAzureStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AzurePhotoStorage>()
        .AddScoped<AddPhotoToMainStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().AddOriginalPhoto)
        .AddScoped<GetOriginalUri>(sp => sp.GetRequiredService<AzurePhotoStorage>().GetOriginalUri)
        .AddScoped<GetThumbnailUri>(sp => sp.GetRequiredService<AzurePhotoStorage>().GetThumbnailUri)
        .AddScoped<ReadOriginalPhoto>(sp => sp.GetRequiredService<AzurePhotoStorage>().ReadOriginalPhoto)
        .AddScoped<AddPhotoToThumbnailStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().AddPhotoToThumbnailStorage);

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
            .AddScoped<OnNewPhotoUploaded>(sp =>
            {
                var serviceBus = sp.GetRequiredService<AzureServiceBus>();
                return serviceBus.OnNewPhotoUploaded;
            });
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

        return services;
    }
}
