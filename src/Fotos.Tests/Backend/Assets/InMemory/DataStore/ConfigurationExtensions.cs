using Fotos.App.Api.Account;
using Fotos.App.Api.PhotoAlbums;
using Fotos.App.Api.PhotoFolders;
using Fotos.App.Api.Photos;
using Fotos.App.Api.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fotos.Tests.Backend.Assets.InMemory.DataStore;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddInMemoryPhotoDataStore(this IServiceCollection services)
    {
        services.AddSingleton<List<Photo>>();
        services.AddSingleton<InMemoryPhotoDataStore>();
        services.AddSingleton<AddPhotoToStore>(sp => sp.GetRequiredService<InMemoryPhotoDataStore>().Add);
        services.AddSingleton<RemovePhotoFromStore>(sp => sp.GetRequiredService<InMemoryPhotoDataStore>().Remove);
        services.AddSingleton<GetPhotoFromStore>(sp => sp.GetRequiredService<InMemoryPhotoDataStore>().Get);
        services.AddSingleton<ListPhotosFromStore>(sp => sp.GetRequiredService<InMemoryPhotoDataStore>().GetByAlbum);
        services.AddSingleton<GetAlbumPhotoCountFromStore>(sp => sp.GetRequiredService<InMemoryPhotoDataStore>().CountPhotos);

        return services;
    }

    public static IServiceCollection AddInMemoryAlbumDataStore(this IServiceCollection services)
    {
        services.AddSingleton<List<Album>>();
        services.AddSingleton<InMemoryAlbumDataStore>();
        services.AddSingleton<AddAlbumToStore>(sp => sp.GetRequiredService<InMemoryAlbumDataStore>().Add);
        services.AddSingleton<GetAlbumFromStore>(sp => sp.GetRequiredService<InMemoryAlbumDataStore>().Get);
        services.AddSingleton<GetFolderAlbumsFromStore>(sp => sp.GetRequiredService<InMemoryAlbumDataStore>().GetByFolder);

        return services;
    }

    public static IServiceCollection AddInMemoryFolderDataStore(this IServiceCollection services)
    {
        services.AddSingleton<List<Folder>>(sp => [Folder.Create(sp.GetService<RootFolderIdWrapper>()?.Value ?? Guid.NewGuid(), Guid.Empty, "Root")]);
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

    public static IServiceCollection SetRootFolderId(this IServiceCollection services, Guid rootFolderId)
    {
        services.TryAddSingleton(new RootFolderIdWrapper(rootFolderId));

        return services;
    }
}

internal sealed class RootFolderIdWrapper
{
    public Guid Value { get; }

    public RootFolderIdWrapper(Guid value) => Value = value;
}