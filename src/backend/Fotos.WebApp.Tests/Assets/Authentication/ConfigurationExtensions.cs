using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.WebApp.Tests.Assets.Authentication;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddFakeAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, _ => { });

        return services;
    }
}
