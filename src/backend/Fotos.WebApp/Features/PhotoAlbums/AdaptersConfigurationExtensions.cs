using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.PhotoAlbums;

internal static class AdaptersConfigurationExtensions
{
    public static IServiceCollection AddPhotoAlbumAdapters(this IServiceCollection services)
    {
        services
            .AddSingleton<List<Album>>(_ => [])
            .AddScoped<GetFolderAlbums>(sp =>
            {
                var store = sp.GetRequiredService<List<Album>>();

                return folderId =>
                {
                    var albums = store.Where(x => x.FolderId == folderId).ToList();

                    return Task.FromResult<IReadOnlyCollection<Album>>(albums);
                };
            })
            .AddScoped<AddAlbum>(sp => album =>
            {
                var store = sp.GetRequiredService<List<Album>>();
                store.Add(album);

                return Task.CompletedTask;
            })
            .AddScoped<GetAlbum>(sp =>
            {
                var store = sp.GetRequiredService<List<Album>>();

                return (albumId) =>
                {
                    var album = store.Single(x => x.Id == albumId.Id);

                    return Task.FromResult(album);
                };
            });

        return services;
    }
}
