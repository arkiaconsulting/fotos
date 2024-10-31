﻿namespace Fotos.Client.Api.Account;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddAccountBusiness(this IServiceCollection services)
    {
        services.AddScoped<AddUserBusiness>();

        return services;
    }
}
