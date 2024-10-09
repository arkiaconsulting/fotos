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

        return services;
    }
}
