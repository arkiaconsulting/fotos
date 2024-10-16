namespace Fotos.WebApp.Features.Photos;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddPhotosBusiness(this IServiceCollection services)
    {
        services.AddScoped<AddPhotosBusiness>();

        return services;
    }
}
