using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.Client.Api.Account;

internal static class Endpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder endpoints, string authenticationScheme)
    {
        var group = endpoints.MapGroup("/account");

        group.MapPost("/login", (HttpContext context, [FromForm] string provider, [FromQuery] string? returnUrl) =>
        {
            returnUrl = Helpers.ComputeSafeReturnUrl(context.Request.PathBase, returnUrl);

            if (!provider.Equals(GoogleDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
            {
                return (IResult)TypedResults.BadRequest($"{provider} is not an recognized authentication provider");
            }

            return Helpers.Challenge(provider, GoogleDefaults.AuthenticationScheme, returnUrl);
        }).AllowAnonymous()
        .DisableAntiforgery()
        .ExcludeFromDescription();

        group.MapGet("/login-callback", async (HttpContext context) =>
        {
            if (context.User.Identity?.IsAuthenticated == false)
            {
                return TypedResults.Redirect("/account/signin");
            }

            var result = await context.AuthenticateAsync();
            var returnUrl = result.Properties?.Items["returnUrl"];
            returnUrl = Helpers.ComputeSafeReturnUrl(context.Request.PathBase, returnUrl);

            await context.SignInAsync(authenticationScheme, context.User);

            return TypedResults.Redirect(returnUrl);
        }).AllowAnonymous()
        .ExcludeFromDescription();

        group.MapPost("/logout", (HttpContext context, [FromForm] string? returnUrl) =>
        {
            returnUrl = Helpers.ComputeSafeReturnUrl(context.Request.PathBase, returnUrl);

            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };

            return TypedResults.SignOut(authenticationProperties, [authenticationScheme]);
        }).AllowAnonymous()
        .DisableAntiforgery()
        .ExcludeFromDescription();

        return endpoints;
    }
}