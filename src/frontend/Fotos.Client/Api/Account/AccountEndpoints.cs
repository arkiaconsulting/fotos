using Fotos.Client.Api.Types;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fotos.Client.Api.Account;

internal static class AccountEndpoints
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

        group.MapGet("/login-callback", async (HttpContext context, [FromServices] FindUserInStore findUser) =>
        {
            if (context.User.Identity?.IsAuthenticated == false || context?.User.Identity is null)
            {
                return (IResult)TypedResults.Redirect("/account/signin");
            }

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return TypedResults.BadRequest($"The claim '{ClaimTypes.NameIdentifier}' was not found");
            }

            var result = await context.AuthenticateAsync();
            var returnUrl = result.Properties?.Items["returnUrl"];
            returnUrl = Helpers.ComputeSafeReturnUrl(context.Request.PathBase, returnUrl);
            var provider = context.User.Identity.AuthenticationType!;

            var user = await findUser(FotoUserId.Create(provider, userId));

            if (user is null)
            {
                return TypedResults.Redirect("/account/register");
            }

            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };

            return TypedResults.SignIn(result.Principal!, authenticationProperties, authenticationScheme);
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

        group.MapGet("/users", () => TypedResults.Ok())
            .AllowAnonymous()
            .DisableAntiforgery()
            .ExcludeFromDescription();

        return endpoints;
    }
}