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
    public async Task Test01(Guid folderId, Guid albumId, Guid photoId, byte[] photoBytes, Uri photoUri)
    {
        _fotoContext.Photos.Add(new PhotoEntity(folderId, albumId, photoId, photoUri));
        _fotoContext.MainStorage.Add((photoId, photoBytes));

        await _fotoContext.ExifMetadataExtractor.Handle(new(folderId, albumId, photoId));

        var photo = _fotoContext.Photos.Should().ContainSingle().Subject;
        (photo.Metadata?.DateTaken).Should().NotBeNull();
    }
}
