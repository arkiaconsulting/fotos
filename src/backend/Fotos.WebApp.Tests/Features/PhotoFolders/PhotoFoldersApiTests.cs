using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.WebApp.Tests.Assets;
using Fotos.WebApp.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.WebApp.Tests.Features.PhotoFolders;

[Trait("Category", "Unit")]
public sealed class PhotoFoldersApiTests : IClassFixture<FotoApi>
{
    private List<Folder> Folders => _fotoApi.Services.GetRequiredService<List<Folder>>();

    private readonly FotoApi _fotoApi;

    public PhotoFoldersApiTests(FotoApi fotoApi) => _fotoApi = fotoApi;

    [Theory(DisplayName = "Creating a new folder with a valid name should pass"), AutoData]
    public async Task Test01(Guid parentFolderId, string folderName)
    {
        var client = _fotoApi.CreateClient();

        using var response = await client.CreatePhotoFolder(parentFolderId, folderName);

        response.Should().Be200Ok();
    }

    [Theory(DisplayName = "Creating a new folder with an invalid payload should fail"), ClassData(typeof(CreateFolderWrongTheoryData))]
    public async Task Test02(string body)
    {
        var client = _fotoApi.CreateClient();

        using var response = await client.CreatePhotoFolderWithBody(body);

        response.Should().Be400BadRequest();
        response.Should().MatchInContent("*https://tools.ietf.org/html/rfc9110#section-15.5.1*");
    }

    [Theory(DisplayName = "Creating a new folder should store it for future use"), AutoData]
    public async Task Test03(Guid parentFolderId, string folderName)
    {
        var client = _fotoApi.CreateClient();

        using var response = await client.CreatePhotoFolder(parentFolderId, folderName);

        Folders.Should().ContainSingle();
    }
}
