using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.WebApp.Features.Photos;
using Fotos.WebApp.Tests.Assets;

namespace Fotos.WebApp.Tests.Features.PhotoFunctions;

[Trait("Category", "Unit")]
public sealed class OnShouldRemovePhotoBinariesHandlerTests : IClassFixture<FotoFunctionsContext>
{
    private readonly FotoFunctionsContext _fotoContext;
    public OnShouldRemovePhotoBinariesHandlerTests(FotoFunctionsContext fotoContext) => _fotoContext = fotoContext;

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
