using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.LoginPath = "/account/signin";
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var issuer = configuration["BaseUrl"] ?? throw new ArgumentNullException("BaseUrl setting is missing or empty", default(Exception));
                var audience = configuration["BaseUrl"] ?? throw new ArgumentNullException("BaseUrl setting is missing or empty", default(Exception));
                var signingKey = configuration["AccessTokenSigningKey"] ?? throw new ArgumentNullException("AccessTokenSigningKey setting is missing or empty", default(Exception));

                options.TokenValidationParameters = new()
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                };
            });

        services.AddAuthorizationBuilder()
            .AddDefaultPolicy(Constants.DefaultPolicy, policy => policy.RequireAuthenticatedUser())
            .AddPolicy(Constants.ApiPolicy, policy => policy.AddAuthenticationSchemes([JwtBearerDefaults.AuthenticationScheme]).RequireAuthenticatedUser());

        services.AddSingleton<AccessTokenService>();

        return services;
    }
}
