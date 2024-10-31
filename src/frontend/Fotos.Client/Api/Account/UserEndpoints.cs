using Fotos.Client.Api.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.Client.Api.Account;

internal static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/users");

        group.MapPost("/", async ([FromBody] FotoUserDto user, [FromServices] AddUserBusiness business, [FromServices] InstrumentationConfig instrumentation) =>
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
        .DisableAntiforgery();

        return endpoints;
    }
}
