using Microsoft.AspNetCore.Authentication.Google;

namespace Fotos.App.Authentication;

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
                options.Cookie.Name = Constants.CookieName;
                options.LoginPath = "/account/signin";
            });

        services.AddAuthorizationBuilder()
            .AddDefaultPolicy(Constants.DefaultPolicy, policy => policy.RequireAuthenticatedUser());

        return services;
    }
}
