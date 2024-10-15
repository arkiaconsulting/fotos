namespace Fotos.Client.Features.PhotoFolders;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddFotosApi(this IServiceCollection services)
    {
        services.AddHttpClient<FotosApiClient>(client => client.BaseAddress = new Uri("https://localhost:7112"));

        services.AddScoped<ListFolders>(sp =>
        {
            var client = sp.GetRequiredService<FotosApiClient>();

            return async (Guid guid) => await client.GetFolders(guid);
        });

        services.AddScoped<CreateFolder>(sp =>
        {
            var client = sp.GetRequiredService<FotosApiClient>();

            return async (Guid parentId, string name) => await client.CreateFolder(parentId, name);
        });

        services.AddScoped<GetFolder>(sp =>
        {
            var client = sp.GetRequiredService<FotosApiClient>();

            return async (Guid folderId) => await client.GetFolder(folderId);
        });

        services.AddScoped<RemoveFolder>(sp =>
        {
            var client = sp.GetRequiredService<FotosApiClient>();

            return async (Guid folderId) => await client.RemoveFolder(folderId);
        });
        services.AddScoped<ListAlbums>(sp =>
        {
            var client = sp.GetRequiredService<FotosApiClient>();

            return async (Guid folderId) => await client.GetAlbums(folderId);
        });
        services.AddScoped<CreateAlbum>(sp =>
        {
            var client = sp.GetRequiredService<FotosApiClient>();

            return async (Guid folderId, string name) => await client.CreateAlbum(folderId, name);
        });
        services.AddScoped<GetAlbum>(sp =>
        {
            var client = sp.GetRequiredService<FotosApiClient>();

            return async (Guid folderId, Guid albumId) => await client.GetAlbum(folderId, albumId);
        });
        services.AddScoped<ListPhotos>(sp =>
        {
            var client = sp.GetRequiredService<FotosApiClient>();

            return async (Guid folderId, Guid albumId) => await client.ListPhotos(folderId, albumId);
        });
        services.AddScoped<AddPhoto>(sp =>
        {
            var client = sp.GetRequiredService<FotosApiClient>();

            return async (Guid folderId, Guid albumId, byte[] buffer) => await client.AddPhoto(folderId, albumId, buffer);
        });
        services.AddScoped<RemovePhoto>(sp =>
        {
            var client = sp.GetRequiredService<FotosApiClient>();

            return async (Guid folderId, Guid albumId, Guid photoId) => await client.RemovePhoto(folderId, albumId, photoId);
        });
        services.AddScoped<GetOriginalUri>(sp =>
        {
            var client = sp.GetRequiredService<FotosApiClient>();

            return async (Guid folderId, Guid albumId, Guid photoId) => await client.GetOriginalUri(folderId, albumId, photoId);
        });

        return services;
    }
}
