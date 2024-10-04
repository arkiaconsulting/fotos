using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.WebApp.Tests.Assets;

namespace Fotos.WebApp.Tests.Features.PhotoAlbums;

[Trait("Category", "Unit")]

public sealed class PhotoAlbumsApiTests : IClassFixture<FotoApi>
{
    private readonly FotoApi _fotoApi;

    public PhotoAlbumsApiTests(FotoApi fotoApi) => _fotoApi = fotoApi;

    [Theory(DisplayName = "Creating an album into a folder should pass"), AutoData]
    public async Task Test01(Guid folderId, string albumName)
    {
        var client = _fotoApi.CreateClient();

        using var response = await client.CreatePhotoAlbum(folderId, albumName);

        response.Should().Be200Ok();
    }

    [Theory(DisplayName = "Creating a new album with an invalid payload should fail"), ClassData(typeof(CreateAlbumWrongTheoryData))]
    public async Task Test02(string body)
    {
        var client = _fotoApi.CreateClient();

        using var response = await client.CreatePhotoAlbumWithBody(body);

        response.Should().Be400BadRequest();
        response.Should().MatchInContent("*https://tools.ietf.org/html/rfc9110#section-15.5.1*");
    }
}
