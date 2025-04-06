using Fotos.App.Api.Framework;
using Fotos.App.Application.User;
using Fotos.App.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fotos.App.Api.Account;

internal static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/users")
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithTags("Account")
            .RequireAuthorization(Constants.ApiPolicy)
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

        return endpoints;
    }
}
