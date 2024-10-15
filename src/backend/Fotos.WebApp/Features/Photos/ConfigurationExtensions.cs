using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.Photos;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddPhotosBusiness(this IServiceCollection services)
    {
        services.AddScoped<AddPhotosBusiness>();

        services.AddScoped<GetOriginalUri>(_ => (PhotoId _) => Task.FromResult(new Uri("img/new.png", UriKind.Relative)));

        return services;
    }
}
