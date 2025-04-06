namespace Fotos.App.Application.Albums;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddAlbumBusiness(this IServiceCollection services)
    {
        services.AddScoped<CreateAlbumBusiness>()
            .AddScoped<ListFolderAlbumsBusiness>()
            .AddScoped<GetAlbumBusiness>();

        return services;
    }
}
