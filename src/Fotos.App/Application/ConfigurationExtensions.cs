using Fotos.App.Application.User;

namespace Fotos.App.Application;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddAccountBusiness();

        return services
            .AddMediator()
            .RegisterMediatorHandlers(typeof(ConfigurationExtensions).Assembly)
            .Services;
    }
}
