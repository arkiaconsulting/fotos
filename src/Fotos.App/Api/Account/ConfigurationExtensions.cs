using Fotos.App.Application.User;

namespace Fotos.App.Api.Account;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddAccountBusiness(this IServiceCollection services)
    {
        services.AddScoped<AddUserBusiness>()
            .AddScoped<FindUserBusiness>();

        return services;
    }
}
