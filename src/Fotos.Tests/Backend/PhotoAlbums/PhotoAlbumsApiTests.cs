using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.App.Api.Photos;
using Fotos.App.Features.Photos;
using Fotos.Tests.Backend.Assets;
using System.Net.Http.Json;

namespace Fotos.Tests.Backend.PhotoAlbums;

[Trait("Category", "Unit")]

public sealed class PhotoAlbumsApiTests : IClassFixture<FotoApi>
{
    private readonly FotoApi _fotoApi;

    public PhotoAlbumsApiTests(FotoApi fotoApi) => _fotoApi = fotoApi;

    [Theory(DisplayName = "Adding a photo into an album should pass"), AutoData]
    public async Task Test03(Guid folderId, Guid albumId, byte[] photo)
    {
        var client = _fotoApi.CreateAuthenticatedClient();

        using var response = await client.AddPhoto(folderId, albumId, photo);

        response.Should().Be202Accepted();
        var id = await response.Content.ReadFromJsonAsync<Guid>();
        id.Should().NotBeEmpty();
        _fotoApi.PhotoUploadedMessageSink.Should().Contain(new PhotoId(folderId, albumId, id));
    }

    [Theory(DisplayName = "Listing the photos of an album should pass"), AutoData]
    internal async Task Test06(PhotoId photoId, string title)
    {
        var client = _fotoApi.CreateAuthenticatedClient();
        _fotoApi.Photos.Add(new Photo(photoId, title));

        using var response = await client.ListPhotos(photoId.FolderId, photoId.AlbumId);

        response.Should().Be200Ok();
        var actual = await response.Content.ReadFromJsonAsync<List<object>>();
        actual.Should().ContainSingle();
    }

    [Theory(DisplayName = "Removing a photo from an album should pass"), AutoData]
    internal async Task Test07(PhotoId photoId, string title)
    {
        var client = _fotoApi.CreateAuthenticatedClient();
        _fotoApi.Photos.Add(new Photo(photoId, title));

        using var response = await client.RemovePhoto(photoId.FolderId, photoId.AlbumId, photoId.Id);

        response.Should().Be204NoContent();
        _fotoApi.PhotoRemovedMessageSink.Should().Contain(photoId);
    }

    [Theory(DisplayName = "Getting the original URI of a photo should pass"), AutoData]
    internal async Task Test08(PhotoId photoId, string title)
    {
        var client = _fotoApi.CreateAuthenticatedClient();
        _fotoApi.Photos.Add(new Photo(photoId, title));

        using var response = await client.GetOriginalUri(photoId.FolderId, photoId.AlbumId, photoId.Id);

        response.Should().Be200Ok();
    }

    [Theory(DisplayName = "Getting the thumbnail URI of a photo should pass"), AutoData]
    internal async Task Test09(PhotoId photoId, string title)
    {
        var client = _fotoApi.CreateAuthenticatedClient();
        _fotoApi.Photos.Add(new Photo(photoId, title));

        using var response = await client.GetThumbnailUri(photoId.FolderId, photoId.AlbumId, photoId.Id);

        response.Should().Be200Ok();
    }

    [Theory(DisplayName = "Updating the title of a photo should pass"), AutoData]
    internal async Task Test10(PhotoId photoId, string title)
    {
        var client = _fotoApi.CreateAuthenticatedClient();
        _fotoApi.Photos.Add(new Photo(photoId, title));

        using var response = await client.UpdatePhoto(photoId.FolderId, photoId.AlbumId, photoId.Id, title);

        response.Should().Be204NoContent();
    }

    [Theory(DisplayName = "Getting a photo should pass"), AutoData]
    internal async Task Test11(PhotoId photoId, string title)
    {
        var client = _fotoApi.CreateAuthenticatedClient();
        _fotoApi.Photos.Add(new Photo(photoId, title));

        using var response = await client.GetPhoto(photoId.FolderId, photoId.AlbumId, photoId.Id);

        response.Should().Be200Ok();
    }
}
