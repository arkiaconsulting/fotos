using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.Tests.Backend.Assets.Authentication;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddFakeAuthentication(this IServiceCollection services)
    {
        services.Configure<AuthenticationOptions>(options =>
        {
            options.SchemeMap.Clear();
            (options.Schemes as IList<AuthenticationSchemeBuilder>)?.Clear();
        });

        services.AddAuthentication()
            .AddScheme<JwtBearerOptions, JwtAuthHandler>(JwtBearerDefaults.AuthenticationScheme, null);

        return services;
    }
}
