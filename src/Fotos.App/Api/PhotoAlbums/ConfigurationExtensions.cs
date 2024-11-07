namespace Fotos.App.Api.PhotoAlbums;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddAlbumsBusiness(this IServiceCollection services)
    {
        services.AddScoped<GetFolderAlbumsBusiness>();
        services.AddScoped<GetAlbumBusiness>();

        return services;
    }
}
