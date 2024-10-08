using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.PhotoFolders;

internal static class AdaptersConfigurationExtensions
{
    public static IServiceCollection AddPhotoFoldersAdapters(this IServiceCollection services)
    {
        services
            .AddSingleton<List<Folder>>()
            .AddScoped<StoreNewFolder>(sp => folder =>
            {
                var store = sp.GetRequiredService<List<Folder>>();
                store.Add(folder);

                return Task.CompletedTask;
            })
            .AddScoped<GetFolders>(sp => parentFolderId =>
            {
                var store = sp.GetRequiredService<List<Folder>>();
                var folders = store.Where(x => x.ParentId == parentFolderId).ToList();

                return Task.FromResult<IReadOnlyCollection<Folder>>(folders);
            });

        return services;
    }
}
