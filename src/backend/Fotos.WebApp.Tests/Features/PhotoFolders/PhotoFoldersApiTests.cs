using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.WebApp.Tests.Assets;
using System.Net.Http.Json;

namespace Fotos.WebApp.Tests.Features.PhotoFolders;

[Trait("Category", "Unit")]
public sealed class PhotoFoldersApiTests : IClassFixture<FotoApi>
{
    private readonly FotoApi _fotoApi;

    public PhotoFoldersApiTests(FotoApi fotoApi) => _fotoApi = fotoApi;

    [Theory(DisplayName = "Creating a new folder with a valid name should pass"), AutoData]
    public async Task Test01(Guid parentFolderId, string folderName)
    {
        var client = _fotoApi.CreateClient();

        using var response = await client.CreatePhotoFolder(parentFolderId, folderName);

        response.Should().Be200Ok();
        (await response.Content.ReadFromJsonAsync<Guid>()).Should().NotBeEmpty();
    }

    [Theory(DisplayName = "Creating a new folder with an invalid payload should fail"), ClassData(typeof(CreateFolderWrongTheoryData))]
    public async Task Test02(string body)
    {
        var client = _fotoApi.CreateClient();

        using var response = await client.CreatePhotoFolderWithBody(body);

        response.Should().Be400BadRequest();
        response.Should().MatchInContent("*https://tools.ietf.org/html/rfc9110#section-15.5.1*");
    }

    [Theory(DisplayName = "Listing folders at root when no child folders should return empty list"), AutoData]
    public async Task Test03(Guid rootFolderId)
    {
        var client = _fotoApi.CreateClient();

        using var response = await client.ListPhotoFolders(rootFolderId);

        var actual = await response.Content.ReadFromJsonAsync<List<object>>();
        actual.Should().BeEmpty();
    }

    [Theory(DisplayName = "Listing folders at root when having child folders should not return other root child folders"), AutoData]
    public async Task Test04(Guid rootFolderId, Guid anotherFolderId, string folderName)
    {
        var client = _fotoApi.CreateClient();
        _ = await client.CreatePhotoFolder(rootFolderId, folderName);
        _ = await client.CreatePhotoFolder(anotherFolderId, folderName);

        using var response = await client.ListPhotoFolders(rootFolderId);

        var actual = await response.Content.ReadFromJsonAsync<List<object>>();
        actual.Should().ContainSingle();
    }

    [Theory(DisplayName = "Getting a folder that does not exist should fail"), AutoData]
    public async Task Test05(Guid parentId, Guid nonExistingFolderId)
    {
        var client = _fotoApi.CreateClient();

        using var response = await client.GetFolder(parentId, nonExistingFolderId);

        response.Should().Be400BadRequest();
        response.Should().MatchInContent("*https://tools.ietf.org/html/rfc9110#section-15.5.1*");
    }

    [Theory(DisplayName = "Removing a folder that does not exist should pass"), AutoData]
    public async Task Test06(Guid parentId, Guid nonExistingFolderId)
    {
        var client = _fotoApi.CreateClient();

        using var response = await client.RemoveFolder(parentId, nonExistingFolderId);

        response.Should().Be204NoContent();
    }

    [Theory(DisplayName = "Removing a folder that exists should pass"), AutoData]
    public async Task Test07(Guid parentId, string folderName)
    {
        var client = _fotoApi.CreateClient();
        using var r0 = await client.CreatePhotoFolder(parentId, folderName);
        var folderId = await r0.Content.ReadFromJsonAsync<Guid>();

        using var response1 = await client.RemoveFolder(parentId, folderId);

        response1.Should().Be204NoContent();
        using var response2 = await client.GetFolder(parentId, folderId);
        response2.Should().Be400BadRequest();
    }
}
