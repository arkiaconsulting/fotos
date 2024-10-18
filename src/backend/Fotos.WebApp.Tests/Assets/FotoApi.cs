using Fotos.WebApp.Features.PhotoAlbums;
using Fotos.WebApp.Features.PhotoFolders;
using Fotos.WebApp.Features.Photos;
using Fotos.WebApp.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.WebApp.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed class FotoApi : WebApplicationFactory<Program>
{
    internal List<PhotoEntity> Photos => Services.GetRequiredService<List<PhotoEntity>>();
    internal List<PhotoId> PhotoRemovedMessageSink { get; } = [];
    internal List<PhotoId> PhotoUploadedMessageSink { get; } = [];

    public FotoApi() => ClientOptions.BaseAddress = new Uri("https://localhost");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureServices(services =>
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
            .AddScoped<GetOriginalUri>((_) => (_) => Task.FromResult(new Uri("https://localhost")))
            .AddScoped<GetThumbnailUri>((_) => (_) => Task.FromResult(new Uri("https://localhost")))
            .AddSingleton<List<PhotoEntity>>(_ => [])
            .AddScoped<StorePhotoData>(_ => entity =>
            {
                Photos.Add(entity);

                return Task.CompletedTask;
            })
            .AddScoped<ListPhotos>(_ => albumId => Task.FromResult<IReadOnlyCollection<PhotoEntity>>(Photos.Where(p => p.Id.FolderId == albumId.FolderId && p.Id.AlbumId == albumId.Id).ToList()))
            .AddScoped<RemovePhotoData>(sp => (photoId) =>
            {
                var store = sp.GetRequiredService<List<PhotoEntity>>();

                store.RemoveAll(photo => photo.Id.Id == photoId.Id);

                return Task.CompletedTask;
            })
            .AddScoped<GetPhoto>(_ => (photoId) =>
            {
                var store = _.GetRequiredService<List<PhotoEntity>>();

                return Task.FromResult(store.Single(x => x.Id.Id == photoId.Id));
            })
            .AddSingleton<List<Album>>(_ => [])
            .AddScoped<GetFolderAlbums>(sp =>
            {
                var store = sp.GetRequiredService<List<Album>>();

                return folderId =>
                {
                    var albums = store.Where(x => x.FolderId == folderId).ToList();

                    return Task.FromResult<IReadOnlyCollection<Album>>(albums);
                };
            })
            .AddScoped<AddAlbum>(sp => album =>
            {
                var store = sp.GetRequiredService<List<Album>>();
                store.Add(album);

                return Task.CompletedTask;
            })
            .AddScoped<GetAlbum>(sp =>
            {
                var store = sp.GetRequiredService<List<Album>>();

                return (albumId) =>
                {
                    var album = store.Single(x => x.Id == albumId.Id);

                    return Task.FromResult(album);
                };
            })
            .AddSingleton<List<Folder>>(_ => [Folder.Create(Guid.NewGuid(), Guid.Empty, "Root")])
            .AddScoped<StoreNewFolder>(sp => folder =>
            {
                var store = sp.GetRequiredService<List<Folder>>();
                store.Add(folder);

                return Task.CompletedTask;
            })
            .AddScoped<GetFolders>(sp => parentFolderId =>
            {
                var store = sp.GetRequiredService<List<Folder>>();
                var folders = store.Where(x => x.ParentId == parentFolderId).ToList();

                return Task.FromResult<IReadOnlyCollection<Folder>>(folders);
            })
            .AddScoped<GetFolder>(sp => (_, folderId) =>
            {
                var store = sp.GetRequiredService<List<Folder>>();

                return Task.FromResult(store.First(x => x.Id == folderId));
            })
            .AddScoped<RemoveFolder>(sp => (_, folderId) =>
            {
                var store = sp.GetRequiredService<List<Folder>>();
                var folder = store.First(x => x.Id == folderId);
                store.Remove(folder);

                return Task.CompletedTask;
            })
        );

        builder.ConfigureAppConfiguration(ConfigureAppConfiguration);
    }

    private static void ConfigureAppConfiguration(WebHostBuilderContext _, IConfigurationBuilder builder) =>
        builder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AzureWebJobs.OnShouldProduceThumbnail.Disabled"] = "true",
            ["AzureWebJobs.OnShouldRemovePhotoBinaries.Disabled"] = "true",
            ["AzureWebJobs.OnShouldExtractExifMetadata.Disabled"] = "true",
        });
}
