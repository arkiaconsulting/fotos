using Fotos.Client.Api.Framework;
using Fotos.Client.Api.Types;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fotos.Client.Api.Account;

internal static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/users");

        group.MapPut("/", async ([FromBody] CreateFotoUserDto user, [FromServices] AddUserBusiness business, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("add user", System.Diagnostics.ActivityKind.Server);

            var (provider, providerUserId, givenName) = user;

            activity?.SetTag("provider", provider);
            activity?.SetTag("providerUserId", providerUserId);

            await business.Process(provider, providerUserId, givenName);

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("user added"));

            return TypedResults.NoContent();
        }).AddEndpointFilter<ValidationEndpointFilter>()
        .AllowAnonymous()
        .WithSummary("Create or update a user")
        .WithTags("Account")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithOpenApi()
        .DisableAntiforgery();

        group.MapGet("/me", async (ClaimsPrincipal principal, [FromServices] FindUserInStore findUserInStore) =>
        {
            var provider = principal.Identity?.AuthenticationType ?? throw new InvalidOperationException("user not authenticated");
            var providerId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var user = (await findUserInStore(FotoUserId.Create(provider, providerId))).Value;

            return TypedResults.Ok(new FotoUserDto(user.GivenName.Value, user.RootFolderId));
        }).RequireAuthorization()
        .WithSummary("Get my user details")
        .WithTags("Account")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithOpenApi();

        return endpoints;
    }
}
