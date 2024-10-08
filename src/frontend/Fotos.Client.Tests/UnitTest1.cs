using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.Client.Components.Pages;
using Fotos.Client.Features.PhotoFolders;
using Fotos.Client.Tests.Assets;

namespace Fotos.Client.Tests;

[Trait("Category", "Unit")]
public sealed class UnitTest1 : IDisposable
{
    private readonly FotosTestContext _testContext;

    public UnitTest1() => _testContext = new FotosTestContext();

    [Theory(DisplayName = "Home page should folders that are children of root"), AutoData]
    public void Test01(string folderName)
    {
        _testContext.Folders.Add(new Folder { ParentFolderId = _testContext.RootFolderId, Name = folderName });
        var home = _testContext.RenderComponent<Home>();

        home.Find("ul").MarkupMatches($"<ul><li diff:ignoreChildren>{folderName}</li></ul>");
    }

    [Theory(DisplayName = "Adding a folder at root should add it to the folders list"), AutoData]
    public void Test02(string folderName)
    {
        var home = _testContext.RenderComponent<Home>();
        var newFolderInput = home.Find("input");
        newFolderInput.Change(folderName);

        home.Find("button").Click();

        home.WaitForAssertion(() => home.FindAll($"ul li:contains('{folderName}')").MarkupMatches("<li diff:ignoreChildren>{folderName}</li>"));
    }

    [Theory(DisplayName = "Clicking on a newly created folder should display its child folders (none in this case)"), AutoData]
    public void Test03(string folderName)
    {
        _testContext.Folders.Add(new Folder { ParentFolderId = _testContext.RootFolderId, Name = folderName });

        var home = _testContext.RenderComponent<Home>();

        home.Find($"ul li:contains('{folderName}') button").Click();

        home.WaitForAssertion(() => home.Find("ul").ChildElementCount.Should().Be(0));
    }

    #region IDisposable

    public void Dispose() => _testContext.Dispose();

    #endregion
}