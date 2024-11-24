using Fotos.App.Adapters;
using Fotos.App.Authentication;
using Fotos.App.Features.Account;
using Fotos.App.Features.PhotoFolders;

namespace Fotos.App.Features;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddFotosApi(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTransient<ClientAuthenticationHandler>()
            .AddHttpClient(Constants.HttpClientName, client => client.BaseAddress = new Uri(configuration["BaseUrl"]!))
            .AddHttpMessageHandler<ClientAuthenticationHandler>();

        services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(Constants.HttpClientName));

        services.AddScoped<FotosApiClient>();
        services.AddHttpContextAccessor();
        services.RegisterImplementation<ListFolders, FotosApiClient>(c => c.GetFolders);
        services.RegisterImplementation<CreateFolder, FotosApiClient>(c => c.CreateFolder);
        services.RegisterImplementation<GetFolder, FotosApiClient>(c => c.GetFolder);
        services.RegisterImplementation<RemoveFolder, FotosApiClient>(c => c.RemoveFolder);
        services.RegisterImplementation<UpdateFolder, FotosApiClient>(c => c.UpdateFolder);
        services.RegisterImplementation<ListAlbums, FotosApiClient>(c => c.GetAlbums);
        services.RegisterImplementation<CreateAlbum, FotosApiClient>(c => c.CreateAlbum);
        services.RegisterImplementation<GetAlbum, FotosApiClient>(c => c.GetAlbum);
        services.RegisterImplementation<ListPhotos, FotosApiClient>(c => c.ListPhotos);
        services.RegisterImplementation<AddPhoto, FotosApiClient>(c => c.AddPhoto);
        services.RegisterImplementation<RemovePhoto, FotosApiClient>(c => c.RemovePhoto);
        services.RegisterImplementation<GetOriginalUri, FotosApiClient>(c => c.GetOriginalUri);
        services.RegisterImplementation<GetThumbnailUri, FotosApiClient>(c => c.GetThumbnailUri);
        services.RegisterImplementation<UpdatePhoto, FotosApiClient>(c => c.UpdatePhoto);
        services.RegisterImplementation<GetPhoto, FotosApiClient>(c => c.GetPhoto);
        services.RegisterImplementation<SaveUser, FotosApiClient>(c => c.SaveUser);
        services.RegisterImplementation<GetMe, FotosApiClient>(c => c.GetMe);

        return services;
    }

    private static IServiceCollection RegisterImplementation<TDelegate, TAdapter>(this IServiceCollection services, Func<TAdapter, TDelegate> implementer)
        where TDelegate : class
        where TAdapter : notnull =>
        services.AddScoped(sp => implementer(sp.GetRequiredService<TAdapter>()));
}
