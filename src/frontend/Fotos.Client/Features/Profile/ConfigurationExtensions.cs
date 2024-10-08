namespace Fotos.Client.Features.Profile;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddProfileFeature(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<GetRootFolderId>(_ => () => Task.FromResult(Guid.Parse(configuration["Profile:RootFolderId"]!)));

        return services;
    }
}
