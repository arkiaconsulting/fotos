namespace Fotos.Client.Api.Photos;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddPhotosBusiness(this IServiceCollection services)
    {
        services.AddScoped<AddPhotosBusiness>();
        services.AddScoped<RemovePhotoBusiness>();
        services.AddScoped<UpdatePhotoBusiness>();

        return services;
    }
}
