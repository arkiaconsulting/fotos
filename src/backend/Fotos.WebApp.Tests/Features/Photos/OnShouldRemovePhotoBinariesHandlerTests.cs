using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.WebApp.Tests.Assets;
using Fotos.WebApp.Types;

namespace Fotos.WebApp.Tests.Features.Photos;

[Trait("Category", "Unit")]
public sealed class OnShouldRemovePhotoBinariesHandlerTests : IClassFixture<FotoContext>
{
    private readonly FotoContext _fotoContext;
    public OnShouldRemovePhotoBinariesHandlerTests(FotoContext fotoContext) => _fotoContext = fotoContext;

    [Theory(DisplayName = "When a photo has been removed, its binaries should be removed as well"), AutoData]
    internal async Task Test01(PhotoId photoId, byte[] photoBytes, byte[] thumbnailBytes)
    {
        _fotoContext.MainStorage.Add((photoId.Id, photoBytes));
        _fotoContext.ThumbnailsStorage.Add((photoId.Id, thumbnailBytes));

        await _fotoContext.OnShouldRemovePhotoBinaries.Handle(photoId);

        _fotoContext.MainStorage.Should().NotContain((photoId.Id, photoBytes));
        _fotoContext.ThumbnailsStorage.Should().NotContain((photoId.Id, thumbnailBytes));
    }
}
