using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.WebApp.Tests.Assets;
using Fotos.WebApp.Types;
using System.Net.Http.Json;
using System.Text.Json;

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

        response.Should().Be204NoContent();
    }

    [Theory(DisplayName = "Creating a new album with an invalid payload should fail"), ClassData(typeof(CreateAlbumWrongTheoryData))]
    internal async Task Test02(string _, string body, string folderId)
    {
        var client = _fotoApi.CreateClient();

        using var response = await client.CreatePhotoAlbumWithBody(Guid.Parse(folderId), body);

        response.Should().Be400BadRequest();
        response.Should().MatchInContent("*https://tools.ietf.org/html/rfc9110#section-15.5.1*");
    }

    [Theory(DisplayName = "Adding a photo into an album should pass"), AutoData]
    public async Task Test03(Guid folderId, Guid albumId, byte[] photo)
    {
        var client = _fotoApi.CreateClient();

        using var response = await client.AddPhoto(folderId, albumId, photo);

        response.Should().Be202Accepted();
        var id = await response.Content.ReadFromJsonAsync<Guid>();
        id.Should().NotBeEmpty();
    }

    [Theory(DisplayName = "Listing folder albums should pass"), AutoData]
    public async Task Test04(Guid folderId, string albumName)
    {
        var client = _fotoApi.CreateClient();
        using var _ = await client.CreatePhotoAlbum(folderId, albumName);

        using var response = await client.ListFolderAlbums(folderId);

        response.Should().Be200Ok();
        var actual = await response.Content.ReadFromJsonAsync<List<object>>();
        actual.Should().ContainSingle();
    }

    [Theory(DisplayName = "Getting a single album should pass"), AutoData]
    public async Task Test05(Guid folderId, string albumName)
    {
        var client = _fotoApi.CreateClient();
        using var _ = await client.CreatePhotoAlbum(folderId, albumName);
        using var _2 = await client.ListFolderAlbums(folderId);
        var actual = await _2.Content.ReadFromJsonAsync<List<JsonElement>>();
        var albumId = actual.Should().ContainSingle().Subject.GetProperty("id").GetGuid();

        using var response = await client.GetAlbum(folderId, albumId);

        response.Should().Be200Ok();
    }

    [Theory(DisplayName = "Listing the photos of an album should pass"), AutoData]
    internal async Task Test06(PhotoId photoId)
    {
        var client = _fotoApi.CreateClient();
        _fotoApi.Photos.Add(new PhotoEntity(photoId));

        using var response = await client.ListPhotos(photoId.FolderId, photoId.AlbumId);

        response.Should().Be200Ok();
        var actual = await response.Content.ReadFromJsonAsync<List<object>>();
        actual.Should().ContainSingle();
    }

    [Theory(DisplayName = "Removing a photo from an album should pass"), AutoData]
    internal async Task Test07(PhotoId photoId)
    {
        var client = _fotoApi.CreateClient();
        _fotoApi.Photos.Add(new PhotoEntity(photoId));

        using var response = await client.RemovePhoto(photoId.FolderId, photoId.AlbumId, photoId.Id);

        response.Should().Be204NoContent();
    }

    [Theory(DisplayName = "Getting the original URI of a photo should pass"), AutoData]
    internal async Task Test08(PhotoId photoId)
    {
        var client = _fotoApi.CreateClient();
        _fotoApi.Photos.Add(new PhotoEntity(photoId));

        using var response = await client.GetOriginalUri(photoId.FolderId, photoId.AlbumId, photoId.Id);

        response.Should().Be200Ok();
    }
}
