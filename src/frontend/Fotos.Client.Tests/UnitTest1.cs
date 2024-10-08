using AutoFixture.Xunit2;
using Fotos.Client.Components.Pages;
using Fotos.Client.Features.PhotoFolders;
using Fotos.Client.Tests.Assets;

namespace Fotos.Client.Tests;

[Trait("Category", "Unit")]
public sealed class UnitTest1 : IClassFixture<FotosTestContext>
{
    private readonly FotosTestContext _testContext;

    public UnitTest1(FotosTestContext testContext) => _testContext = testContext;

    [Theory(DisplayName = "Home page should folders that are children of root"), AutoData]
    public void Test01(string folderName)
    {
        _testContext.Folders.Add(new Folder { ParentId = _testContext.RootFolderId, Name = folderName });
        var home = _testContext.RenderComponent<Home>();

        home.Find("ul").MarkupMatches($"<ul><li>{folderName}</li></ul>");
    }
}