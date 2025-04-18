﻿namespace Fotos.App.Application.User;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddAccountBusiness(this IServiceCollection services)
    {
        services.AddScoped<AddUserBusiness>()
            .AddScoped<FindUserBusiness>();

        return services;
    }
}
