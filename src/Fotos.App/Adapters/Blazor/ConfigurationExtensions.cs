using Fotos.App.Application.User;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Fotos.App.Adapters.Blazor;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddCustomCircuitHandler(this IServiceCollection services) =>
        services.AddScoped<CustomCircuitHandler>()
        .AddScoped<CircuitHandler>(sp => sp.GetRequiredService<CustomCircuitHandler>())
        .AddScoped(sp => sp.GetRequiredService<CustomCircuitHandler>().SessionData)
        .AddSingleton<List<SessionData>>(_ => []);

}
