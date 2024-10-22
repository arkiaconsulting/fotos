using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.Client.Api.Photos;
using Fotos.WebApp.Tests.Assets;

namespace Fotos.WebApp.Tests.Features.PhotoFunctions;

[Trait("Category", "Unit")]
public sealed class OnNewPhotoUploadedHandlerTests : IClassFixture<FotoFunctionsContext>
{
    private readonly FotoFunctionsContext _fotoContext;
    public OnNewPhotoUploadedHandlerTests(FotoFunctionsContext fotoContext) => _fotoContext = fotoContext;

    [Theory(DisplayName = "When a new photo has been uploaded, its EXIF metadata should be stored"), AutoData]
    internal async Task Test01(PhotoId photoId, byte[] photoBytes, string title)
    {
        _fotoContext.Photos.Add(new Photo(photoId, title));
        _fotoContext.MainStorage.Add((photoId.Id, photoBytes));

        await _fotoContext.ExifMetadataExtractor.Handle(photoId);

        var photo = _fotoContext.Photos.Should().ContainSingle(e => e.Id == photoId).Subject;
        (photo.Metadata?.DateTaken).Should().NotBeNull();
    }

    [Theory(DisplayName = "When a new photo has been uploaded, its thumbnail representation should be stored"), AutoData]
    internal async Task Test02(PhotoId photoId, byte[] photoBytes, string title)
    {
        _fotoContext.Photos.Add(new Photo(photoId, title));
        _fotoContext.MainStorage.Add((photoId.Id, photoBytes));

        await _fotoContext.OnShouldProduceThumbnail.Handle(photoId);

        _fotoContext.ThumbnailsStorage.Should().ContainSingle(x => x.Item1 == photoId.Id);
    }

    [Theory(DisplayName = "When a thumbnail has been generated, should inform by using a message"), AutoData]
    internal async Task Test03(PhotoId photoId, byte[] photoBytes, string title)
    {
        _fotoContext.Photos.Add(new Photo(photoId, title));
        _fotoContext.MainStorage.Add((photoId.Id, photoBytes));

        await _fotoContext.OnShouldProduceThumbnail.Handle(photoId);

        _fotoContext.ThumbnailsReady.Should().ContainSingle(p => p == photoId);
    }
}
