using Fotos.App.Application.Photos;

namespace Fotos.App.Adapters.Imaging;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddImageProcessing(this IServiceCollection services)
    {
        services.AddSingleton<SkiaSharpImageProcessing>();
        services.AddScoped<CreateThumbnail>(_ => SkiaSharpImageProcessing.CreateThumbnail);
        services.AddSingleton<ExifMetadataService>();
        services.AddScoped<ExtractExifMetadata>(_ => ExifMetadataService.Extract);

        return services;
    }
}
