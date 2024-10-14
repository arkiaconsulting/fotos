using Fotos.WebApp.Features.Photos;
using Fotos.WebApp.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.ObjectModel;

namespace Fotos.WebApp.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed class FotoContext
{
    internal Collection<(Guid, byte[])> MainStorage => _host.Services.GetRequiredService<Collection<(Guid, byte[])>>();
    internal Collection<PhotoEntity> Photos => _host.Services.GetRequiredService<Collection<PhotoEntity>>();
    internal OnShouldExtractExifMetadata ExifMetadataExtractor => _host.Services.GetRequiredService<OnShouldExtractExifMetadata>();

    private readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureServices(ConfigureServices)
        .Build();

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<Collection<(Guid, byte[])>>();
        services.AddSingleton<Collection<PhotoEntity>>();
        services.AddSingleton<OnShouldExtractExifMetadata>();
        services.AddSingleton<ReadOriginalPhoto>(sp =>
        {
            var mainStorage = sp.GetRequiredService<Collection<(Guid, byte[])>>();

            return photoId =>
            {
                var (id, bytes) = mainStorage.Single(x => x.Item1 == photoId);

                return Task.FromResult<Stream>(new MemoryStream(bytes));
            };
        });
        services.AddSingleton<ExtractExifMetadata>((_) => (_) => Task.FromResult(new ExifMetadata(DateTime.Now)));
        services.AddSingleton<GetPhoto>(_ => (folderId, albumId, photoId) =>
        {
            var store = _.GetRequiredService<Collection<PhotoEntity>>();

            return Task.FromResult(store.Single(x => x.FolderId == folderId && x.AlbumId == albumId && x.Id == photoId));
        });
        services.AddSingleton<StorePhotoData>(sp =>
        {
            var store = sp.GetRequiredService<Collection<PhotoEntity>>();

            return photo =>
            {
                var existingPhoto = store.FirstOrDefault(x => x.Id == photo.Id);
                if (existingPhoto != null)
                {
                    store.Remove(existingPhoto);
                }

                store.Add(photo);

                return Task.CompletedTask;
            };
        });
    }
}
