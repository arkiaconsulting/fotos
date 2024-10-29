using Fotos.Client.Api.PhotoAlbums;
using Fotos.Client.Api.PhotoFolders;
using Fotos.Client.Api.Photos;
using Fotos.Client.Features.Photos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.WebApp.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed class FotoApi : WebApplicationFactory<Program>
{
    internal List<Photo> Photos => Services.GetRequiredService<List<Photo>>();
    internal List<PhotoId> PhotoRemovedMessageSink { get; } = [];
    internal List<PhotoId> PhotoUploadedMessageSink { get; } = [];

    public FotoApi() => ClientOptions.BaseAddress = new Uri("https://localhost");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureServices(services =>
        {
            services.AddScoped<AddPhotoToMainStorage>(_ => (_, _, _) => Task.CompletedTask)
            .AddScoped<OnNewPhotoUploaded>((_) => (id) =>
            {
                PhotoUploadedMessageSink.Add(id);

                return Task.CompletedTask;
            })
            .AddScoped<OnPhotoRemoved>((_) => (id) =>
            {
                PhotoRemovedMessageSink.Add(id);

                return Task.CompletedTask;
            })
            .AddScoped<GetOriginalStorageUri>((_) => (_) => Task.FromResult(new Uri("https://localhost")))
            .AddScoped<GetThumbnailStorageUri>((_) => (_) => Task.FromResult(new Uri("https://localhost")))
            .AddSingleton<List<Photo>>(_ => [])
            .AddScoped<AddPhotoToStore>(_ => entity =>
            {
                Photos.Add(entity);

                return Task.CompletedTask;
            })
            .AddScoped<ListPhotosFromStore>(_ => albumId => Task.FromResult<IReadOnlyCollection<Photo>>(Photos.Where(p => p.Id.FolderId == albumId.FolderId && p.Id.AlbumId == albumId.Id).ToList()))
            .AddScoped<RemovePhotoFromStore>(sp => (photoId) =>
            {
                var store = sp.GetRequiredService<List<Photo>>();

                store.RemoveAll(photo => photo.Id.Id == photoId.Id);

                return Task.CompletedTask;
            })
            .AddScoped<GetPhotoFromStore>(_ => (photoId) =>
            {
                var store = _.GetRequiredService<List<Photo>>();

                return Task.FromResult(store.Single(x => x.Id.Id == photoId.Id));
            })
            .AddSingleton<List<Album>>(_ => [])
            .AddScoped<GetFolderAlbumsFromStore>(sp =>
            {
                var store = sp.GetRequiredService<List<Album>>();

                return folderId =>
                {
                    var albums = store.Where(x => x.FolderId == folderId).ToList();

                    return Task.FromResult<IReadOnlyCollection<Album>>(albums);
                };
            })
            .AddScoped<AddAlbumToStore>(sp => album =>
            {
                var store = sp.GetRequiredService<List<Album>>();
                store.Add(album);

                return Task.CompletedTask;
            })
            .AddScoped<GetAlbumFromStore>(sp =>
            {
                var store = sp.GetRequiredService<List<Album>>();

                return (albumId) =>
                {
                    var album = store.Single(x => x.Id == albumId.Id);

                    return Task.FromResult(album);
                };
            })
            .AddSingleton<List<Folder>>(_ => [Folder.Create(Guid.NewGuid(), Guid.Empty, "Root")])
            .AddScoped<AddFolderToStore>(sp => folder =>
            {
                var store = sp.GetRequiredService<List<Folder>>();
                store.Add(folder);

                return Task.CompletedTask;
            })
            .AddScoped<GetFoldersFromStore>(sp => parentFolderId =>
            {
                var store = sp.GetRequiredService<List<Folder>>();
                var folders = store.Where(x => x.ParentId == parentFolderId).ToList();

                return Task.FromResult<IReadOnlyCollection<Folder>>(folders);
            })
            .AddScoped<GetFolderFromStore>(sp => (_, folderId) =>
            {
                var store = sp.GetRequiredService<List<Folder>>();

                return Task.FromResult(store.First(x => x.Id == folderId));
            })
            .AddScoped<RemoveFolderFromStore>(sp => (_, folderId) =>
            {
                var store = sp.GetRequiredService<List<Folder>>();
                var folder = store.First(x => x.Id == folderId);
                store.Remove(folder);

                return Task.CompletedTask;
            })
            .AddScoped<UpdateFolderInStore>(sp => (parent, id, name) =>
            {
                var store = sp.GetRequiredService<List<Folder>>();
                var existing = store.First(x => x.Id == id);
                store.Remove(existing);

                store.Add(new(id, parent, name));

                return Task.CompletedTask;
            });
        });

        builder.ConfigureAppConfiguration(ConfigureAppConfiguration);
    }

    private static void ConfigureAppConfiguration(WebHostBuilderContext _, IConfigurationBuilder builder) =>
        builder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AzureWebJobs.OnShouldProduceThumbnail.Disabled"] = "true",
            ["AzureWebJobs.OnShouldRemovePhotoBinaries.Disabled"] = "true",
            ["AzureWebJobs.OnShouldExtractExifMetadata.Disabled"] = "true",
            ["AzureWebJobs.OnThumbnailReady.Disabled"] = "true",
        });

    public override async ValueTask DisposeAsync()
    {
        // Due to pipelines runs transient failures
        try
        {
            await base.DisposeAsync();
        }
        catch (ObjectDisposedException)
        {
            // OK
        }
    }
}
