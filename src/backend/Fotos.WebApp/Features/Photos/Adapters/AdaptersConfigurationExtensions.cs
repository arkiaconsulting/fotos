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
            .AddScoped<OnNewPhotoUploaded>((_) => (_) => Task.CompletedTask)
            .AddScoped<ListPhotos>(sp =>
            {
                var store = sp.GetRequiredService<List<PhotoEntity>>();

                return (albumId) =>
                {
                    var photos = store.Where(x => x.Id.AlbumId == albumId.Id).ToList();

                    return Task.FromResult<IReadOnlyCollection<PhotoEntity>>(photos);
                };
            })
            .AddScoped<RemovePhoto>(sp => (photoId) =>
            {
                var store = sp.GetRequiredService<List<PhotoEntity>>();

                store.RemoveAll(photo => photo.Id.Id == photoId.Id);

                return Task.CompletedTask;
            })
            .AddScoped<ReadOriginalPhoto>((_) => (_) => Task.FromResult(Stream.Null))
            .AddScoped<ExtractExifMetadata>((_) => (_) => Task.FromResult(new ExifMetadata(DateTime.Now)))
            .AddScoped<GetPhoto>(_ => (photoId) =>
            {
                var store = _.GetRequiredService<List<PhotoEntity>>();

                return Task.FromResult(store.Single(x => x.Id.Id == photoId.Id));
            })
            .AddScoped<CreateThumbnail>((_) => (_) => Task.FromResult(Stream.Null))
            .AddScoped<AddPhotoToThumbnailStorage>((_) => (_, _) => Task.CompletedTask);

        services.AddFotosAzureStorage(configuration);

        return services;
    }

    public static IServiceCollection AddFotosAzureStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AzurePhotoStorage>()
            .AddScoped<AddPhotoToMainStorage>(sp =>
        {
            var photoStorage = sp.GetRequiredService<AzurePhotoStorage>();

            return photoStorage.AddOriginalPhoto;
        });

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
}
