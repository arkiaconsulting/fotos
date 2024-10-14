using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.Photos;

internal static class AdaptersConfigurationExtensions
{
    public static IServiceCollection AddPhotosAdapters(this IServiceCollection services)
    {
        services
            .AddSingleton<List<PhotoEntity>>(_ => [])
            .AddScoped<StorePhotoData>(sp => photo =>
            {
                var store = sp.GetRequiredService<List<PhotoEntity>>();
                store.Add(photo);

                return Task.CompletedTask;
            })
            .AddScoped<AddPhotoToMainStorage>((_) => (_, _) => Task.CompletedTask)
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

        return services;
    }
}
