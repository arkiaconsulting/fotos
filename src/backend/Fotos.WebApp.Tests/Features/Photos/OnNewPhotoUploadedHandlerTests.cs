using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.WebApp.Tests.Assets;
using Fotos.WebApp.Types;

namespace Fotos.WebApp.Tests.Features.Photos;

[Trait("Category", "Unit")]
public sealed class OnNewPhotoUploadedHandlerTests : IClassFixture<FotoContext>
{
    private readonly FotoContext _fotoContext;
    public OnNewPhotoUploadedHandlerTests(FotoContext fotoContext) => _fotoContext = fotoContext;

    [Theory(DisplayName = "When a new photo has been uploaded, its EXIF metadata should be stored"), AutoData]
    internal async Task Test01(PhotoId photoId, byte[] photoBytes, string title)
    {
        _fotoContext.Photos.Add(new PhotoEntity(photoId, title));
        _fotoContext.MainStorage.Add((photoId.Id, photoBytes));

        await _fotoContext.ExifMetadataExtractor.Handle(photoId);

        var photo = _fotoContext.Photos.Should().ContainSingle().Subject;
        (photo.Metadata?.DateTaken).Should().NotBeNull();
    }

    [Theory(DisplayName = "When a new photo has been uploaded, its thumbnail representation should be stored"), AutoData]
    internal async Task Test02(PhotoId photoId, byte[] photoBytes, string title)
    {
        _fotoContext.Photos.Add(new PhotoEntity(photoId, title));
        _fotoContext.MainStorage.Add((photoId.Id, photoBytes));

        await _fotoContext.ThumbnailProducer.Handle(photoId);

        _fotoContext.ThumbnailsStorage.Should().ContainSingle();
    }
}
