namespace Fotos.App.Application.Photos;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddPhotoBusiness(this IServiceCollection services)
    {
        services.AddScoped<AddPhotosBusiness>()
            .AddScoped<RemovePhotoBusiness>()
            .AddScoped<UpdatePhotoBusiness>()
            .AddScoped<GetPhotoBusiness>()
            .AddScoped<ListAlbumPhotosBusiness>()
            .AddScoped<GetOriginalPhotoUriBusiness>()
            .AddScoped<GetPhotoThumbnailUriBusiness>()
            .AddScoped<AddPhotosBusiness>();

        return services;
    }
}
