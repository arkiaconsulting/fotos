using Fotos.Client.Api.Framework;
using Fotos.Client.Api.Types;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fotos.Client.Api.Account;

internal static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/users")
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithTags("Account")
            .RequireAuthorization(Authentication.Constants.DefaultPolicy)
            .WithOpenApi();

        group.MapPut("/", async (ClaimsPrincipal principal, [FromBody] CreateFotoUserDto user, [FromServices] AddUserBusiness business, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("add user", System.Diagnostics.ActivityKind.Server);

            if (principal.Identity?.IsAuthenticated == false)
            {
                return (IResult)TypedResults.Unauthorized();
            }

            var provider = principal.Identity?.AuthenticationType;
            var providerUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            activity?.SetTag("provider", provider);

            await business.Process(provider!, providerUserId!, user.GivenName);

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("user added"));

            return TypedResults.NoContent();
        })
        .WithSummary("Create or update the authenticated user")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .DisableAntiforgery();

        group.MapGet("/me", async (ClaimsPrincipal principal, [FromServices] FindUserInStore findUserInStore, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("retrieve user details", System.Diagnostics.ActivityKind.Server);

            var provider = principal.Identity?.AuthenticationType ?? throw new InvalidOperationException("user not authenticated");
            var providerId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var user = (await findUserInStore(FotoUserId.Create(provider, providerId))).Value;

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("user details retrieved"));

            return TypedResults.Ok(new FotoUserDto(user.GivenName.Value, user.RootFolderId));
        })
        .WithSummary("Get the authenticated user details")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        return endpoints;
    }
}
