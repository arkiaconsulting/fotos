using Microsoft.AspNetCore.Authentication.Google;

namespace Fotos.Client.Authentication;

internal static class AuthenticationConfigurationExtensions
{
    public static IServiceCollection AddFotosAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCascadingAuthenticationState();

        services.AddAuthentication(Constants.AuthenticationScheme)
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                configuration.Bind("Google", options);

                options.SignInScheme = Constants.AuthenticationScheme;
                options.UsePkce = true;
            })
            .AddCookie(Constants.AuthenticationScheme, options =>
            {
                options.Cookie.Name = ".fotos.user";
                options.LoginPath = "/account/signin";
            });

        return services;
    }
}
