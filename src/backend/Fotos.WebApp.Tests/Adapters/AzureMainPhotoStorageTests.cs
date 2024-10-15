using AutoFixture.Xunit2;
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

        await _context.AddPhotoToMainStorage(photoId, ms);

        var blob = _context.PhotosContainer.GetBlobClient($"{photoId.Id}.original");
        (await blob.ExistsAsync()).Value.Should().BeTrue();
    }
}
