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
            });

        return services;
    }
}
