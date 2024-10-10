using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.Client.Components.Pages;
using Fotos.Client.Features.PhotoFolders;
using Fotos.Client.Tests.Assets;

namespace Fotos.Client.Tests;

[Trait("Category", "Unit")]
public sealed class HomePageTests : IDisposable
{
    private readonly FotosTestContext _testContext;

    public HomePageTests() => _testContext = new FotosTestContext();

    [Theory(DisplayName = "Home page should folders that are children of root"), AutoData]
    public void Test01(string folderName)
    {
        _testContext.Folders.Add(new Folder { Id = Guid.NewGuid(), ParentId = _testContext.RootFolderId, Name = folderName });
        var home = _testContext.RenderComponent<Home>();

        home.Find("ul").MarkupMatches($"<ul><li diff:ignoreChildren>{folderName}</li></ul>");
    }

    [Theory(DisplayName = "Adding a folder at root should add it to the folders list"), AutoData]
    public void Test02(string folderName)
    {
        var home = _testContext.RenderComponent<Home>();
        var newFolderInput = home.Find("input");
        newFolderInput.Change(folderName);

        home.Find("#create-folder").Click();

        home.WaitForAssertion(() => home.FindAll($"ul li:contains('{folderName}')").MarkupMatches("<li diff:ignoreChildren>{folderName}</li>"));
    }

    [Theory(DisplayName = "Clicking on a newly created folder should display its child folders (none in this case)"), AutoData]
    public void Test03(string folderName)
    {
        _testContext.Folders.Add(new Folder { Id = Guid.NewGuid(), ParentId = _testContext.RootFolderId, Name = folderName });

        var home = _testContext.RenderComponent<Home>();

        home.Find($"ul li:contains('{folderName}') #go").Click();

        home.WaitForAssertion(() => home.Find("ul").ChildElementCount.Should().Be(0));
    }

    [Theory(DisplayName = "Navigating to the parent folder should pass"), AutoData]
    public void Test04(string folderName)
    {
        _testContext.Folders.Add(new Folder { Id = Guid.NewGuid(), ParentId = _testContext.RootFolderId, Name = folderName });

        var home = _testContext.RenderComponent<Home>();
        home.Find($"ul li:contains('{folderName}') #go").Click();

        home.Find("#up").Click();
        home.WaitForState(() => true, TimeSpan.FromSeconds(1));

        home.WaitForAssertion(() => home.FindAll($"ul li:contains('{folderName}')").MarkupMatches("<li diff:ignoreChildren>{folderName}</li>"));
    }

    [Fact(DisplayName = "When at root it should not be possible to go to parent")]
    public void Test05()
    {
        var home = _testContext.RenderComponent<Home>();

        home.Find("#up").GetAttribute("disabled").Should().NotBeNull();
    }

    [Theory(DisplayName = "When on a child folder, it should be possible to go to parent"), AutoData]
    public void Test06(string folderName)
    {
        _testContext.Folders.Add(new Folder { Id = Guid.NewGuid(), ParentId = _testContext.RootFolderId, Name = folderName });

        var home = _testContext.RenderComponent<Home>();

        home.Find($"ul li:contains('{folderName}') #go").Click();

        home.Find("#up").GetAttribute("disabled").Should().BeNull();
    }

    [Fact(DisplayName = "The name of the current folder should be displayed")]
    public void Test07()
    {
        var home = _testContext.RenderComponent<Home>();

        home.Find("#current-folder-name").TextContent.MarkupMatches("Root");
    }

    [Theory(DisplayName = "Deleting a folder should remove it from list"), AutoData]
    public void Test08(string folderName)
    {
        _testContext.Folders.Add(new Folder { Id = Guid.NewGuid(), ParentId = _testContext.RootFolderId, Name = folderName });

        var home = _testContext.RenderComponent<Home>();

        home.Find($"ul li:contains('{folderName}') #remove").Click();

        home.WaitForAssertion(() => home.Find("ul").MarkupMatches("<ul></ul>"));
    }

    [Theory(DisplayName = "Creating an empty album in a folder should display it"), AutoData]
    public void Test09(string albumName)
    {
        var home = _testContext.RenderComponent<Home>();

        var newFolderInput = home.Find("#new-album-name");
        newFolderInput.Change(albumName);
        home.Find("#create-album").Click();

        home.WaitForAssertion(() => home.Find("#albums ul").MarkupMatches($"<ul><li>{albumName}</li></ul>"));
    }

    #region IDisposable

    public void Dispose() => _testContext.Dispose();

    #endregion
}