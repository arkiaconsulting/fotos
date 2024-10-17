namespace Fotos.WebApp.Features.Photos;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddPhotosBusiness(this IServiceCollection services)
    {
        services.AddScoped<AddPhotosBusiness>();
        services.AddScoped<RemovePhotoBusiness>();

        return services;
    }
}
