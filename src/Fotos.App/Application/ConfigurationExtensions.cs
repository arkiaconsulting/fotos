using Fotos.App.Application.User;

namespace Fotos.App.Application;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<SessionDataStorage>();

        return services
            .AddMediator()
            .RegisterMediatorHandlers(typeof(ConfigurationExtensions).Assembly)
            .Services;
    }
}
