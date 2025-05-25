using Microsoft.Extensions.DependencyInjection;

namespace Fotos.Application;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddMediator()
            .RegisterMediatorHandlers(typeof(ConfigurationExtensions).Assembly)
            .Services;
    }
}
