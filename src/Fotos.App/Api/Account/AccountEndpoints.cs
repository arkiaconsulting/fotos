using Fotos.App.Application.User;
using Fotos.App.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fotos.App.Api.Account;

internal static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/account")
            .ExcludeFromDescription();

        group.MapPost("/login", (HttpContext context, [FromForm] string provider, [FromForm] string? returnUrl) =>
        {
            returnUrl = Helpers.ComputeSafeReturnUrl(context.Request.PathBase, returnUrl);

            if (!provider.Equals(GoogleDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
            {
                return (IResult)TypedResults.BadRequest($"{provider} is not an recognized authentication provider");
            }

            return Helpers.Challenge(provider, GoogleDefaults.AuthenticationScheme, returnUrl);
        }).AllowAnonymous();

        group.MapGet("/login-callback", async Task<IResult> (HttpContext context, [FromServices] FindUserInStore findUser) =>
        {
            var externalAuthResult = await context.AuthenticateAsync(Authentication.Constants.ExternalAuthenticationScheme);

            if (externalAuthResult.Principal?.Identity?.IsAuthenticated == false || context?.User.Identity is null)
            {
                return TypedResults.Redirect("/account/signin");
            }

            var externalPrincipal = externalAuthResult.Principal!;
            var userId = externalPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return TypedResults.BadRequest($"The claim '{ClaimTypes.NameIdentifier}' was not found");
            }

            var returnUrl = externalAuthResult.Properties?.RedirectUri;
            returnUrl = Helpers.ComputeSafeReturnUrl(context.Request.PathBase, returnUrl);
            var provider = externalAuthResult.Principal!.Identity!.AuthenticationType;

            var user = await findUser(FotoUserId.Create(provider!, userId));

            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = $"/account/externallogin?returnurl={returnUrl}",
            };

            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(externalPrincipal.Claims, provider));

            if (user is null)
            {
                await context.SignInAsync(principal, authenticationProperties);

                return TypedResults.Redirect("/account/register");
            }

            return TypedResults.SignIn(principal, authenticationProperties);
        })
        .AllowAnonymous();

        group.MapPost("/logout", (HttpContext context, [FromForm] string? returnUrl) =>
        {
            returnUrl = Helpers.ComputeSafeReturnUrl(context.Request.PathBase, returnUrl);

            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };

            return TypedResults.SignOut(authenticationProperties, [CookieAuthenticationDefaults.AuthenticationScheme, Authentication.Constants.ExternalAuthenticationScheme]);
        }).RequireAuthorization();

        return endpoints;
    }
}