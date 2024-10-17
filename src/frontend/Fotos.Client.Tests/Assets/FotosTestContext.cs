using Fotos.Client.Features.PhotoFolders;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.Client.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed class FotosTestContext : TestContext
{
    internal List<Folder> Folders => Services.GetRequiredService<List<Folder>>();
    internal List<Album> Albums => Services.GetRequiredService<List<Album>>();
    internal List<Photo> Photos => Services.GetRequiredService<List<Photo>>();

    public Guid RootFolderId { get; } = Guid.NewGuid();

    public FotosTestContext() => ConfigureServices();

    private void ConfigureServices()
    {
        Services.AddSingleton<List<Folder>>(_ => [new Folder(RootFolderId, Guid.Empty, "Root")]);
        Services.AddTransient<ListFolders>(sp =>
        {
            var folders = sp.GetRequiredService<List<Folder>>();

            return (Guid guid) => Task.FromResult<IReadOnlyCollection<Folder>>(folders.Where(f => f.ParentId == guid).ToList());
        });
        Services.AddTransient<CreateFolder>(sp =>
        {
            var folders = sp.GetRequiredService<List<Folder>>();

            return (Guid parentId, string name) => Task.Run(() => folders.Add(new Folder(Guid.NewGuid(), parentId, name)));
        });
        Services.AddTransient<GetFolder>(sp =>
        {
            var folders = sp.GetRequiredService<List<Folder>>();

            return (Guid folderId) => Task.Run(() => folders.Single(f => f.Id == folderId));
        });
        Services.AddTransient<RemoveFolder>(sp =>
        {
            var folders = sp.GetRequiredService<List<Folder>>();

            return (Guid folderId) => Task.Run(() =>
            {
                var folder = folders.First(f => f.Id == folderId);

                folders.Remove(folder);
            });
        });
        Services.AddSingleton<List<Album>>(_ => []);
        Services.AddTransient<CreateAlbum>(sp =>
        {
            var albums = sp.GetRequiredService<List<Album>>();

            return (Guid folderId, string name) => Task.Run(() => albums.Add(new Album(Guid.NewGuid(), folderId, name)));
        });
        Services.AddTransient<ListAlbums>(sp =>
        {
            var albums = sp.GetRequiredService<List<Album>>();

            return (Guid folderId) => Task.FromResult<IReadOnlyCollection<Album>>(albums.Where(a => a.FolderId == folderId).ToList());
        });
        Services.AddTransient<GetAlbum>(sp =>
        {
            var albums = sp.GetRequiredService<List<Album>>();

            return (Guid _, Guid albumId) => Task.Run(() => albums.Single(a => a.Id == albumId));
        });
        Services.AddSingleton<List<Photo>>(_ => []);
        Services.AddTransient<ListPhotos>(sp =>
        {
            var photos = sp.GetRequiredService<List<Photo>>();

            return (Guid _, Guid albumId) => Task.FromResult<IReadOnlyCollection<Photo>>(photos.Where(p => p.AlbumId == albumId).ToList());
        });
        Services.AddTransient<AddPhoto>(sp =>
        {
            var photos = sp.GetRequiredService<List<Photo>>();

            return (Guid folderId, Guid albumId, PhotoBinary _) => Task.Run(() =>
            {
                photos.Add(new(Guid.NewGuid(), folderId, albumId, default!, new Uri("/", UriKind.Relative)));

                return Guid.NewGuid();
            });
        });
        Services.AddTransient<RemovePhoto>(sp =>
        {
            var photos = sp.GetRequiredService<List<Photo>>();

            return (Guid _, Guid _, Guid photoId) => Task.Run(() =>
            {
                var photo = photos.First(p => p.Id == photoId);
                photos.Remove(photo);
            });
        });
        Services.AddTransient<GetOriginalUri>(_ => (Guid _, Guid _, Guid photoId) => Task.FromResult(new Uri($"http://localhost/photos/{photoId}")));
        Services.AddTransient<GetThumbnailUri>(_ => (Guid _, Guid _, Guid photoId) => Task.FromResult<Uri>(new Uri($"http://localhost/photos/{photoId}")));
    }
}