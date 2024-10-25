using Fotos.Client.Adapters;
using Fotos.Client.Features.PhotoAlbums;
using Fotos.Client.Features.PhotoFolders;
using Fotos.Client.Features.Photos;
using Fotos.Client.Hubs;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;

namespace Fotos.Client.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed class FotosTestContext : TestContext
{
    internal List<FolderDto> Folders => Services.GetRequiredService<List<FolderDto>>();
    internal List<AlbumDto> Albums => Services.GetRequiredService<List<AlbumDto>>();
    internal List<PhotoDto> Photos => Services.GetRequiredService<List<PhotoDto>>();

    public Guid RootFolderId { get; } = Guid.NewGuid();

    public FotosTestContext() => ConfigureServices();

    private void ConfigureServices()
    {
        JSInterop.SetupVoid("mudScrollManager.lockScroll", "body", "scroll-locked");
        JSInterop.SetupVoid("mudScrollManager.unlockScroll", "body", "scroll-locked");
        Services.AddMudServices();
        Services.AddSingleton<List<FolderDto>>(_ => [new FolderDto(RootFolderId, Guid.Empty, "Root")]);
        Services.AddTransient<ListFolders>(sp =>
        {
            var folders = sp.GetRequiredService<List<FolderDto>>();

            return (Guid guid) => Task.FromResult<IReadOnlyCollection<FolderDto>>(folders.Where(f => f.ParentId == guid).ToList());
        });
        Services.AddTransient<CreateFolder>(sp =>
        {
            var folders = sp.GetRequiredService<List<FolderDto>>();

            return (Guid parentId, string name) => Task.Run(() => folders.Add(new FolderDto(Guid.NewGuid(), parentId, name)));
        });
        Services.AddTransient<GetFolder>(sp =>
        {
            var folders = sp.GetRequiredService<List<FolderDto>>();

            return (_, folderId) => Task.Run(() => folders.Single(f => f.Id == folderId));
        });
        Services.AddTransient<RemoveFolder>(sp =>
        {
            var folders = sp.GetRequiredService<List<FolderDto>>();

            return (_, folderId) => Task.Run(() =>
            {
                var folder = folders.First(f => f.Id == folderId);

                folders.Remove(folder);
            });
        });
        Services.AddSingleton<List<AlbumDto>>(_ => []);
        Services.AddTransient<CreateAlbum>(sp =>
        {
            var albums = sp.GetRequiredService<List<AlbumDto>>();

            return (Guid folderId, string name) => Task.Run(() => albums.Add(new AlbumDto(Guid.NewGuid(), folderId, name)));
        });
        Services.AddTransient<ListAlbums>(sp =>
        {
            var albums = sp.GetRequiredService<List<AlbumDto>>();

            return (Guid folderId) => Task.FromResult<IReadOnlyCollection<AlbumDto>>(albums.Where(a => a.FolderId == folderId).ToList());
        });
        Services.AddTransient<GetAlbum>(sp =>
        {
            var albums = sp.GetRequiredService<List<AlbumDto>>();

            return albumId => Task.Run(() => albums.Single(a => a.Id == albumId.Id));
        });
        Services.AddSingleton<List<PhotoDto>>(_ => []);
        Services.AddTransient<ListPhotos>(sp =>
        {
            var photos = sp.GetRequiredService<List<PhotoDto>>();

            return albumId => Task.FromResult<IReadOnlyCollection<PhotoDto>>(photos.Where(p => p.AlbumId == albumId.Id).ToList());
        });
        Services.AddTransient<AddPhoto>(sp =>
        {
            var photos = sp.GetRequiredService<List<PhotoDto>>();

            return (AlbumId albumId, PhotoBinary _) => Task.Run(() =>
            {
                photos.Add(new(Guid.NewGuid(), albumId.FolderId, albumId.Id, default!));

                return Guid.NewGuid();
            });
        });
        Services.AddTransient<RemovePhoto>(sp =>
        {
            var photos = sp.GetRequiredService<List<PhotoDto>>();

            return photoId => Task.Run(() =>
            {
                var photo = photos.First(p => p.Id == photoId.Id);
                photos.Remove(photo);
            });
        });
        Services.AddTransient<UpdatePhoto>(sp =>
        {
            var photos = sp.GetRequiredService<List<PhotoDto>>();

            return (photoId, title) =>
            {
                var photo = photos.Single(p => p.Id == photoId.Id);
                photos.Remove(photo);
                photos.Add(new PhotoDto(photo.Id, photo.FolderId, photo.AlbumId, title));

                return Task.CompletedTask;
            };
        });
        Services.AddTransient<GetOriginalUri>(_ => _ => Task.FromResult(new Uri("/", UriKind.Relative)));
        Services.AddTransient<GetThumbnailUri>(_ => _ => Task.FromResult(new Uri("/", UriKind.Relative)));
        Services.AddSingleton<RealTimeMessageService, RealTimeServiceFake>();
    }
}
