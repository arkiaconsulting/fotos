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

                return (folderId, albumId) =>
                {
                    var photos = store.Where(x => x.FolderId == folderId && x.AlbumId == albumId).ToList();

                    return Task.FromResult<IReadOnlyCollection<PhotoEntity>>(photos);
                };
            })
            .AddScoped<RemovePhoto>(sp => (_, _, id) =>
            {
                var store = sp.GetRequiredService<List<PhotoEntity>>();

                store.RemoveAll(photo => photo.Id == id);

                return Task.CompletedTask;
            })
            .AddScoped<ReadOriginalPhoto>((_) => (_) => Task.FromResult(Stream.Null))
            .AddScoped<ExtractExifMetadata>((_) => (_) => Task.FromResult(new ExifMetadata(DateTime.Now)))
            .AddScoped<GetPhoto>(_ => (folderId, albumId, photoId) =>
            {
                var store = _.GetRequiredService<List<PhotoEntity>>();

                return Task.FromResult(store.Single(x => x.FolderId == folderId && x.AlbumId == albumId && x.Id == photoId));
            });

        return services;
    }
}
