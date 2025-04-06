using Fotos.App.Application.Folders;
using Fotos.App.Application.Photos;
using Fotos.App.Application.User;
using Fotos.App.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.Tests.Assets.InMemory.DataStore;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddInMemoryPhotoDataStore(this IServiceCollection services, List<Photo>? photos = default)
    {
        services.AddSingleton(photos ?? []);
        services.AddSingleton<InMemoryPhotoDataStore>();
        services.AddSingleton<AddPhotoToStore>(sp => sp.GetRequiredService<InMemoryPhotoDataStore>().Add);
        services.AddSingleton<RemovePhotoFromStore>(sp => sp.GetRequiredService<InMemoryPhotoDataStore>().Remove);
        services.AddSingleton<GetPhotoFromStore>(sp => sp.GetRequiredService<InMemoryPhotoDataStore>().Get);
        services.AddSingleton<ListPhotosFromStore>(sp => sp.GetRequiredService<InMemoryPhotoDataStore>().GetByAlbum);
        services.AddSingleton<GetAlbumPhotoCountFromStore>(sp => sp.GetRequiredService<InMemoryPhotoDataStore>().CountPhotos);

        return services;
    }

    public static IServiceCollection AddInMemoryAlbumDataStore(this IServiceCollection services, List<Album>? albums = default)
    {
        services.AddSingleton(albums ?? []);
        services.AddSingleton<InMemoryAlbumDataStore>();
        services.AddSingleton<AddAlbumToStore>(sp => sp.GetRequiredService<InMemoryAlbumDataStore>().Add);
        services.AddSingleton<GetAlbumFromStore>(sp => sp.GetRequiredService<InMemoryAlbumDataStore>().Get);
        services.AddSingleton<GetFolderAlbumsFromStore>(sp => sp.GetRequiredService<InMemoryAlbumDataStore>().GetByFolder);

        return services;
    }

    public static IServiceCollection AddInMemoryFolderDataStore(this IServiceCollection services, List<Folder>? folders = default)
    {
        services.AddSingleton(folders ?? []);
        services.AddSingleton<InMemoryFolderDataStore>();
        services.AddSingleton<AddFolderToStore>(sp => sp.GetRequiredService<InMemoryFolderDataStore>().Add);
        services.AddSingleton<RemoveFolderFromStore>(sp => sp.GetRequiredService<InMemoryFolderDataStore>().Remove);
        services.AddSingleton<GetFolderFromStore>(sp => sp.GetRequiredService<InMemoryFolderDataStore>().Get);
        services.AddSingleton<GetFoldersFromStore>(sp => sp.GetRequiredService<InMemoryFolderDataStore>().GetByParent);
        services.AddSingleton<UpdateFolderInStore>(sp => sp.GetRequiredService<InMemoryFolderDataStore>().Update);

        return services;
    }

    public static IServiceCollection AddInMemoryUserDataStore(this IServiceCollection services, List<FotoUser>? users = default)
    {
        services.AddSingleton(users ?? []);
        services.AddSingleton<InMemoryUserDataStore>();
        services.AddSingleton<AddUserToStore>(sp => sp.GetRequiredService<InMemoryUserDataStore>().Add);
        services.AddSingleton<FindUserInStore>(sp => sp.GetRequiredService<InMemoryUserDataStore>().Find);

        return services;
    }
}