﻿using Fotos.Client.Api.Account;
using Fotos.Client.Api.PhotoAlbums;
using Fotos.Client.Api.PhotoFolders;
using Fotos.Client.Api.Photos;
using Fotos.Client.Api.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.WebApp.Tests.Assets.InMemory.DataStore;

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
        services.AddSingleton<List<Folder>>([Folder.Create(Guid.NewGuid(), Guid.Empty, "Root")]);
        services.AddSingleton<InMemoryFolderDataStore>();
        services.AddSingleton<AddFolderToStore>(sp => sp.GetRequiredService<InMemoryFolderDataStore>().Add);
        services.AddSingleton<RemoveFolderFromStore>(sp => sp.GetRequiredService<InMemoryFolderDataStore>().Remove);
        services.AddSingleton<GetFolderFromStore>(sp => sp.GetRequiredService<InMemoryFolderDataStore>().Get);
        services.AddSingleton<GetFoldersFromStore>(sp => sp.GetRequiredService<InMemoryFolderDataStore>().GetByParent);
        services.AddSingleton<UpdateFolderInStore>(sp => sp.GetRequiredService<InMemoryFolderDataStore>().Update);

        return services;
    }

    public static IServiceCollection AddInMemoryUserDataStore(this IServiceCollection services)
    {
        services.AddSingleton<List<FotoUser>>();
        services.AddSingleton<InMemoryUserDataStore>();
        services.AddSingleton<AddUserToStore>(sp => sp.GetRequiredService<InMemoryUserDataStore>().Add);
        services.AddSingleton<FindUserInStore>(sp => sp.GetRequiredService<InMemoryUserDataStore>().Find);

        return services;
    }
}
