using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.Core;
using Fotos.Tests.Assets;

namespace Fotos.Tests.Adapters;

[Trait("Category", "Integration")]
public sealed class AzurePhotoStorageTests : IClassFixture<FotoIntegrationContext>
{
    private readonly FotoIntegrationContext _context;

    public AzurePhotoStorageTests(FotoIntegrationContext context) => _context = context;

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

        uri.AbsoluteUri.Should().StartWith($"https://127.0.0.1:10000/devstoreaccount1/fotostests/{photoId.Id}.original");
    }

    [Theory(DisplayName = "When reading an original photo should return it as stream"), AutoData]
    internal async Task Test03(PhotoId photoId, byte[] photo)
    {
        await using var ms = new MemoryStream(photo);
        var blob = _context.PhotosContainer.GetBlobClient($"{photoId.Id}.original");
        await blob.UploadAsync(ms);

        var (stream, _) = await _context.ReadOriginalPhoto(photoId);

        await using var ms2 = new MemoryStream();
        await stream.CopyToAsync(ms2);
        ms2.Position = 0;
        ms2.ToArray().Should().BeEquivalentTo(photo);
    }

    [Theory(DisplayName = "When producing thumbnail should return it as stream")]
    [InlineData("Adapters/test-file.jpg")]
    internal async Task Test04(string photoTestPath)
    {
        await using var stream = File.OpenRead(photoTestPath);

        await using var thumbnailStream = await _context.CreateThumbnail(new(stream, "image/jpeg"));

        thumbnailStream.Should().NotBeNull();
    }

    [Theory(DisplayName = "When adding a image to the thumbnail storage should effectively store it"), AutoData]
    internal async Task Test05(PhotoId photoId, byte[] photo)
    {
        await using var ms = new MemoryStream(photo);

        await _context.AddPhotoToThumbnailStorage(photoId, new(ms, "image/jpeg"));

        var blob = _context.PhotosContainer.GetBlobClient($"{photoId.Id}.thumbnail");
        var blobContent = await blob.DownloadContentAsync();
        blobContent.Value.Content.ToArray().Should().BeEquivalentTo(photo);
        blobContent.Value.Details.ContentType.Should().Be("image/jpeg");
    }

    [Theory(DisplayName = "When getting the URI of the photo thumbnail should return the storage URI"), AutoData]
    internal async Task Test06(PhotoId photoId, byte[] photo)
    {
        await using var ms = new MemoryStream(photo);
        var blob = _context.PhotosContainer.GetBlobClient($"{photoId.Id}.thumbnail");
        await blob.UploadAsync(ms);

        var uri = await _context.GetThumbnailUri(photoId);

        uri.AbsoluteUri.Should().StartWith($"https://127.0.0.1:10000/devstoreaccount1/fotostests/{photoId.Id}.thumbnail");
    }

    [Theory(DisplayName = "When removing a photo from the main storage should effectively remove it"), AutoData]
    internal async Task Test07(PhotoId photoId, byte[] photo)
    {
        await using var ms = new MemoryStream(photo);
        var blob = _context.PhotosContainer.GetBlobClient($"{photoId.Id}.original");
        await blob.UploadAsync(ms);

        await _context.RemovePhotoOriginal(photoId);

        (await blob.ExistsAsync()).Value.Should().BeFalse();
    }

    [Theory(DisplayName = "When removing a photo from the thumbnail storage should effectively remove it"), AutoData]
    internal async Task Test08(PhotoId photoId, byte[] photo)
    {
        await using var ms = new MemoryStream(photo);
        var blob = _context.PhotosContainer.GetBlobClient($"{photoId.Id}.thumbnail");
        await blob.UploadAsync(ms);

        await _context.RemovePhotoThumbnail(photoId);

        (await blob.ExistsAsync()).Value.Should().BeFalse();
    }
}
