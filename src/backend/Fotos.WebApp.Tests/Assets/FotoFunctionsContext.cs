﻿using Fotos.Client.Api.Photos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.ObjectModel;

namespace Fotos.WebApp.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed class FotoFunctionsContext
{
    internal Collection<(Guid, byte[])> MainStorage => _host.Services.GetRequiredKeyedService<Collection<(Guid, byte[])>>("main");
    internal Collection<(Guid, byte[])> ThumbnailsStorage => _host.Services.GetRequiredKeyedService<Collection<(Guid, byte[])>>("thumbnails");
    internal Collection<Photo> Photos => _host.Services.GetRequiredService<Collection<Photo>>();
    internal OnShouldExtractExifMetadata ExifMetadataExtractor => _host.Services.GetRequiredService<OnShouldExtractExifMetadata>();
    internal OnShouldProduceThumbnail OnShouldProduceThumbnail => _host.Services.GetRequiredService<OnShouldProduceThumbnail>();
    internal OnShouldRemovePhotoBinaries OnShouldRemovePhotoBinaries => _host.Services.GetRequiredService<OnShouldRemovePhotoBinaries>();
    internal Collection<PhotoId> ThumbnailsReady => _host.Services.GetRequiredKeyedService<Collection<PhotoId>>("thumbnailsready");

    private readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureServices(ConfigureServices)
        .ConfigureAppConfiguration(ConfigureAppConfiguration)
        .Build();

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddKeyedSingleton<Collection<(Guid, byte[])>>("main");
        services.AddKeyedSingleton<Collection<(Guid, byte[])>>("thumbnails");
        services.AddKeyedSingleton<Collection<PhotoId>>("thumbnailsready");
        services.AddSingleton<Collection<Photo>>();
        services.AddSingleton<OnShouldExtractExifMetadata>();
        services.AddSingleton<OnShouldProduceThumbnail>();
        services.AddSingleton<OnShouldRemovePhotoBinaries>();
        services.AddSingleton<ReadOriginalPhotoFromStorage>(sp =>
        {
            var mainStorage = sp.GetRequiredKeyedService<Collection<(Guid, byte[])>>("main");

            return photoId =>
            {
                var (id, bytes) = mainStorage.Single(x => x.Item1 == photoId.Id);

                return Task.FromResult<PhotoBinary>(new(new MemoryStream(bytes), "image/jpeg"));
            };
        });
        services.AddSingleton<ExtractExifMetadata>((_) => (_, _) => Task.FromResult(new ExifMetadata(DateTime.Now)));
        services.AddSingleton<GetPhotoFromStore>(_ => (photoId) =>
        {
            var store = _.GetRequiredService<Collection<Photo>>();

            return Task.FromResult(store.Single(x => x.Id.Id == photoId.Id));
        });
        services.AddSingleton<AddPhotoToStore>(sp =>
        {
            var store = sp.GetRequiredService<Collection<Photo>>();

            return photo =>
            {
                var existingPhoto = store.FirstOrDefault(x => x.Id == photo.Id);
                if (existingPhoto != default)
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
        services.AddSingleton<RemovePhotoOriginalFromStorage>(sp => photoId =>
        {
            var mainStorage = sp.GetRequiredKeyedService<Collection<(Guid, byte[])>>("main");
            var photo = mainStorage.Single(x => x.Item1 == photoId.Id);
            mainStorage.Remove(photo);

            return Task.CompletedTask;
        });
        services.AddSingleton<RemovePhotoThumbnailFromStorage>(sp => photoId =>
        {
            var thumbnailsStorage = sp.GetRequiredKeyedService<Collection<(Guid, byte[])>>("thumbnails");
            var photo = thumbnailsStorage.Single(x => x.Item1 == photoId.Id);
            thumbnailsStorage.Remove(photo);

            return Task.CompletedTask;
        });
        services.AddSingleton<OnThumbnailReady>(sp => photoId =>
        {
            var thumbnailsReady = sp.GetRequiredKeyedService<Collection<PhotoId>>("thumbnailsready");
            thumbnailsReady.Add(photoId);

            return Task.CompletedTask;
        });
    }

    private static void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder) =>
        builder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AzureWebJobs.OnShouldProduceThumbnail.Disabled"] = "true"
        });
}
