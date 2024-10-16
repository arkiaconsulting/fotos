using Fotos.WebApp.Features.Photos;
using Fotos.WebApp.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.ObjectModel;

namespace Fotos.WebApp.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed class FotoContext
{
    internal Collection<(Guid, byte[])> MainStorage => _host.Services.GetRequiredKeyedService<Collection<(Guid, byte[])>>("main");
    internal Collection<(Guid, byte[])> ThumbnailsStorage => _host.Services.GetRequiredKeyedService<Collection<(Guid, byte[])>>("thumbnails");
    internal Collection<PhotoEntity> Photos => _host.Services.GetRequiredService<Collection<PhotoEntity>>();
    internal OnShouldExtractExifMetadata ExifMetadataExtractor => _host.Services.GetRequiredService<OnShouldExtractExifMetadata>();
    internal OnShouldProduceThumbnail ThumbnailProducer => _host.Services.GetRequiredService<OnShouldProduceThumbnail>();

    private readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureServices(ConfigureServices)
        .ConfigureAppConfiguration(ConfigureAppConfiguration)
        .Build();

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddKeyedSingleton<Collection<(Guid, byte[])>>("main");
        services.AddKeyedSingleton<Collection<(Guid, byte[])>>("thumbnails");
        services.AddSingleton<Collection<PhotoEntity>>();
        services.AddSingleton<OnShouldExtractExifMetadata>();
        services.AddSingleton<OnShouldProduceThumbnail>();
        services.AddSingleton<ReadOriginalPhoto>(sp =>
        {
            var mainStorage = sp.GetRequiredKeyedService<Collection<(Guid, byte[])>>("main");

            return photoId =>
            {
                var (id, bytes) = mainStorage.Single(x => x.Item1 == photoId.Id);

                return Task.FromResult<PhotoBinary>(new(new MemoryStream(bytes), "image/jpeg"));
            };
        });
        services.AddSingleton<ExtractExifMetadata>((_) => (_) => Task.FromResult(new ExifMetadata(DateTime.Now)));
        services.AddSingleton<GetPhoto>(_ => (photoId) =>
        {
            var store = _.GetRequiredService<Collection<PhotoEntity>>();

            return Task.FromResult(store.Single(x => x.Id.Id == photoId.Id));
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
        services.AddSingleton<CreateThumbnail>((_) => async (originalPhoto) =>
        {
            var thumbnail = new MemoryStream();
            await originalPhoto.Content.CopyToAsync(thumbnail);
            thumbnail.Position = 0;

            return thumbnail;
        });
        services.AddSingleton<AddPhotoToThumbnailStorage>((_) => (photoId, thumbnail) =>
        {
            var thumbnailsStorage = _.GetRequiredKeyedService<Collection<(Guid, byte[])>>("thumbnails");
            thumbnailsStorage.Add((photoId.Id, ((MemoryStream)thumbnail.Content).ToArray()));

            return Task.CompletedTask;
        });
    }

    private static void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder) =>
        builder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AzureWebJobs.OnShouldProduceThumbnail.Disabled"] = "true"
        });
}
