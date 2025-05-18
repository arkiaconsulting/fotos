using Microsoft.AspNetCore.ResponseCompression;

namespace Fotos.App.Adapters.RealTimeMessaging;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddSignalRFotosHub(this IServiceCollection services)
    {
        services.AddSignalR();

        services.AddResponseCompression(options => options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]))
        .AddTransient<RealTimeMessageService>();

        return services;
    }
}
