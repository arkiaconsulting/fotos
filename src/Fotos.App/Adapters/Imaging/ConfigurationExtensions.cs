using Fotos.App.Application.Photos;

namespace Fotos.App.Adapters.Imaging;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddImageProcessing(this IServiceCollection services)
    {
        services.AddSingleton<SkiaSharpImageProcessing>();
        services.AddScoped<CreateThumbnail>(sp => sp.GetRequiredService<SkiaSharpImageProcessing>().CreateThumbnail);
        services.AddSingleton<ExifMetadataService>();
        services.AddScoped<ExtractExifMetadata>(sp => sp.GetRequiredService<ExifMetadataService>().Extract);

        return services;
    }
}
