﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace Fotos.App.Authentication;

internal static class AuthenticationConfigurationExtensions
{
    public static IServiceCollection AddFotosAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCascadingAuthenticationState();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
        {
            configuration.Bind(GoogleDefaults.AuthenticationScheme, options);

            options.UsePkce = true;

            options.Events = new()
            {
                //OnRedirectToAuthorizationEndpoint = context =>
                //{
                //    context.RedirectUri += "&prompt=select_account";
                //    context.Response.Redirect(context.RedirectUri);

                //    return Task.CompletedTask;
                //},
                OnTicketReceived = async context =>
                {
                    var nameIdentifier = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                    var givenName = context.Principal?.FindFirstValue(ClaimTypes.GivenName);

                    context.Principal = new ClaimsPrincipal(
                        new ClaimsIdentity(context.Principal?.Claims, GoogleDefaults.AuthenticationScheme));

                    //await Task.CompletedTask;
                    await context.HttpContext.SignInAsync(Constants.ExternalAuthenticationScheme, context.Principal!, context.Properties);

                    context.HandleResponse();
                    context.Response.Redirect("/account/login-callback");
                }
            };
        }).AddCookie(Constants.ExternalAuthenticationScheme)
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.Name = Constants.CookieName;
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.ExpireTimeSpan = TimeSpan.FromDays(15);
            options.SlidingExpiration = false;
            options.LoginPath = "/account/signin";
            options.AccessDeniedPath = "/account/signin";

            options.Events.OnValidatePrincipal = context =>
            {
                var accessTokenExpirationText = context.Properties.GetTokenValue("expires_at");
                if (!DateTimeOffset.TryParse(accessTokenExpirationText, out var accessTokenExpiration))
                {
                    return Task.CompletedTask;
                }

                var now = options.TimeProvider!.GetUtcNow();
                if (now + TimeSpan.FromMinutes(5) < accessTokenExpiration)
                {
                    return Task.CompletedTask;
                }

                context.ShouldRenew = true;
                var accessTokenService = context.HttpContext.RequestServices.GetRequiredService<AccessTokenService>();
                var accessToken = accessTokenService.GenerateAccessToken(context.Principal!.FindFirstValue(ClaimTypes.NameIdentifier)!, context.Principal!.FindFirstValue(ClaimTypes.GivenName)!);

                context.Properties.StoreFotosApiToken(accessToken);

                return Task.CompletedTask;
            };
        });

        services.AddAuthorizationBuilder()
            .AddDefaultPolicy(Constants.DefaultPolicy, policy => policy.RequireAuthenticatedUser())
            .AddPolicy(Constants.ApiPolicy, policy => policy.AddAuthenticationSchemes([JwtBearerDefaults.AuthenticationScheme]).RequireAuthenticatedUser());

        services.AddSingleton<AccessTokenService>();

        return services;
    }
}
