namespace Fotos.WebApp.Features.Photos;

internal static class AdaptersConfigurationExtensions
{
    public static IServiceCollection AddPhotosAdapters(this IServiceCollection services)
    {
        services
            .AddSingleton<List<Photo>>(_ => [])
            .AddScoped<StorePhotoData>(sp => photo =>
            {
                var store = sp.GetRequiredService<List<Photo>>();
                store.Add(photo);

                return Task.CompletedTask;
            })
            .AddScoped<AddPhotoToMainStorage>((_) => (_, _) => Task.CompletedTask)
            .AddScoped<OnNewPhotoUploaded>((_) => (_) => Task.CompletedTask)
            .AddScoped<ListPhotos>(sp =>
            {
                var store = sp.GetRequiredService<List<Photo>>();

                return (folderId, albumId) =>
                {
                    var photos = store.Where(x => x.FolderId == folderId && x.AlbumId == albumId).ToList();

                    return Task.FromResult<IReadOnlyCollection<Photo>>(photos);
                };
            })
            .AddScoped<RemovePhoto>(sp => (_, _, id) =>
            {
                var store = sp.GetRequiredService<List<Photo>>();

                store.RemoveAll(photo => photo.Id == id);

                return Task.CompletedTask;
            });

        return services;
    }
}
