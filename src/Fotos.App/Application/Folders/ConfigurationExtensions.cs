namespace Fotos.App.Application.Folders;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddFolderBusiness(this IServiceCollection services)
    {
        services.AddScoped<GetFolderBusiness>()
            .AddScoped<RemoveFolderBusiness>()
            .AddScoped<UpdateFolderBusiness>()
            .AddScoped<ListChildFoldersBusiness>()
            .AddScoped<CreateFolderBusiness>();

        return services;
    }
}
