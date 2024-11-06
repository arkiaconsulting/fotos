using Fotos.App.Features.Account;
using Fotos.App.Features.PhotoFolders;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.Tests.Frontend.Assets.InMemory.Api;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddInMemoryFoldersApi(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryFoldersApi>();
        services.AddSingleton<CreateFolder>(sp => sp.GetRequiredService<InMemoryFoldersApi>().Add);
        services.AddSingleton<ListFolders>(sp => sp.GetRequiredService<InMemoryFoldersApi>().List);
        services.AddSingleton<GetFolder>(sp => sp.GetRequiredService<InMemoryFoldersApi>().Get);
        services.AddSingleton<RemoveFolder>(sp => sp.GetRequiredService<InMemoryFoldersApi>().Remove);
        services.AddSingleton<UpdateFolder>(sp => sp.GetRequiredService<InMemoryFoldersApi>().Update);

        return services;
    }

    public static IServiceCollection AddInMemoryAlbumsApi(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryAlbumsApi>();
        services.AddSingleton<CreateAlbum>(sp => sp.GetRequiredService<InMemoryAlbumsApi>().Add);
        services.AddSingleton<ListAlbums>(sp => sp.GetRequiredService<InMemoryAlbumsApi>().List);
        services.AddSingleton<GetAlbum>(sp => sp.GetRequiredService<InMemoryAlbumsApi>().Get);

        return services;
    }

    public static IServiceCollection AddInMemoryPhotosApi(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryPhotosApi>();
        services.AddSingleton<AddPhoto>(sp => sp.GetRequiredService<InMemoryPhotosApi>().Add);
        services.AddSingleton<ListPhotos>(sp => sp.GetRequiredService<InMemoryPhotosApi>().List);
        services.AddSingleton<RemovePhoto>(sp => sp.GetRequiredService<InMemoryPhotosApi>().Remove);
        services.AddSingleton<GetPhoto>(sp => sp.GetRequiredService<InMemoryPhotosApi>().Get);
        services.AddSingleton<UpdatePhoto>(sp => sp.GetRequiredService<InMemoryPhotosApi>().Update);
        services.AddSingleton<GetOriginalUri>(sp => sp.GetRequiredService<InMemoryPhotosApi>().GetOriginalUri);
        services.AddSingleton<GetThumbnailUri>(sp => sp.GetRequiredService<InMemoryPhotosApi>().GetThumbnailUri);

        return services;
    }

    public static IServiceCollection AddInMemoryUsersApi(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryUsersApi>();
        services.AddSingleton<SaveUser>(sp => sp.GetRequiredService<InMemoryUsersApi>().Add);
        services.AddSingleton<GetMe>(sp => sp.GetRequiredService<InMemoryUsersApi>().GetMe);

        return services;
    }
}
