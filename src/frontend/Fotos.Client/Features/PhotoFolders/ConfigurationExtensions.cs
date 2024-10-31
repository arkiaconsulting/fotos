using Fotos.Client.Adapters;
using Fotos.Client.Features.Account;

namespace Fotos.Client.Features.PhotoFolders;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddFotosApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<FotosApiClient>(client => client.BaseAddress = new Uri(configuration["BaseUrl"]!));

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

        return services;
    }

    private static IServiceCollection RegisterImplementation<TDelegate, TAdapter>(this IServiceCollection services, Func<TAdapter, TDelegate> implementer)
        where TDelegate : class
        where TAdapter : notnull =>
        services.AddScoped(sp => implementer(sp.GetRequiredService<TAdapter>()));
}
