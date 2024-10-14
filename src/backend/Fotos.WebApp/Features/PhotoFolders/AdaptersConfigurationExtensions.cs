namespace Fotos.WebApp.Features.PhotoFolders;

internal static class AdaptersConfigurationExtensions
{
    public static IServiceCollection AddPhotoFoldersAdapters(this IServiceCollection services)
    {
        services
            .AddSingleton<List<Folder>>(_ => [Folder.Create(Guid.NewGuid(), Guid.Empty, "Root")])
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
            })
            .AddScoped<GetFolder>(sp => folderId =>
            {
                var store = sp.GetRequiredService<List<Folder>>();

                return Task.FromResult(store.First(x => x.Id == folderId));
            })
            .AddScoped<RemoveFolder>(sp => folderId =>
            {
                var store = sp.GetRequiredService<List<Folder>>();
                var folder = store.First(x => x.Id == folderId);
                store.Remove(folder);

                return Task.CompletedTask;
            });

        return services;
    }
}
