﻿using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.WebApp.Tests.Assets;
using Fotos.WebApp.Types;

namespace Fotos.WebApp.Tests.Adapters;

[Trait("Category", "Unit")]
public sealed class AzureMainPhotoStorageTests : IClassFixture<FotoIntegrationContext>
{
    private readonly FotoIntegrationContext _context;

    public AzureMainPhotoStorageTests(FotoIntegrationContext context) => _context = context;

    [Theory(DisplayName = "When adding a new photo to the main storage should effectively store it"), AutoData]
    internal async Task Test01(PhotoId photoId, byte[] photo)
    {
        await using var ms = new MemoryStream(photo);
        const string contentType = "image/jpeg";
        await _context.AddPhotoToMainStorage(photoId, ms, contentType);

        var blob = _context.PhotosContainer.GetBlobClient($"{photoId.Id}.original");
        var blobContent = await blob.DownloadContentAsync();
        blobContent.Value.Details.ContentType.Should().Be(contentType);
    }

    [Theory(DisplayName = "When getting the URI of the photo original should return the storage URI"), AutoData]
    internal async Task Test02(PhotoId photoId, byte[] photo)
    {
        await using var ms = new MemoryStream(photo);
        var blob = _context.PhotosContainer.GetBlobClient($"{photoId.Id}.original");
        await blob.UploadAsync(ms);

        var uri = await _context.GetOriginalUri(photoId);

        uri.AbsoluteUri.Should().StartWith($"http://127.0.0.1:10000/devstoreaccount1/fotostests/{photoId.Id}.original");
    }
}
