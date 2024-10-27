using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.Client.Adapters;
using Fotos.Client.Components.Pages;
using Fotos.Client.Tests.Assets;

namespace Fotos.Client.Tests;

[Trait("Category", "Unit")]
public sealed class HomePageTests : IDisposable
{
    private readonly FotosTestContext _testContext;

    public HomePageTests() => _testContext = new FotosTestContext();

    [Theory(DisplayName = "Home page should display folders that are children of root"), AutoData]
    public void Test01(string folderName)
    {
        _testContext.Folders.Add(new FolderDto { Id = Guid.NewGuid(), ParentId = _testContext.RootFolderId, Name = folderName });
        var home = _testContext.RenderComponent<Home>();

        home.WaitForElement("#folders .folder .title").InnerHtml.MarkupMatches(folderName);
    }

    [Theory(DisplayName = "Adding a folder at root should add it to the folders list"), AutoData]
    public void Test02(string folderName)
    {
        var home = _testContext.RenderComponent<Home>();
        var newFolderInput = home.Find("input");
        newFolderInput.Input(folderName);

        home.WaitForElement("#create-folder").Click();

        home.WaitForElement($"#folders .folder .title:contains('{folderName}')").InnerHtml.MarkupMatches(folderName);
    }

    [Theory(DisplayName = "Clicking on a newly created folder should display its child folders (none in this case)"), AutoData]
    public void Test03(string folderName)
    {
        _testContext.Folders.Add(new FolderDto { Id = Guid.NewGuid(), ParentId = _testContext.RootFolderId, Name = folderName });

        var home = _testContext.RenderComponent<Home>();

        home.WaitForElement("#folders .folder").MouseOver();
        home.WaitForElement("#folders .folder #go").Click();

        home.WaitForElement("#folders").ChildElementCount.Should().Be(0);
    }

    [Theory(DisplayName = "Navigating to the parent folder should pass"), AutoData]
    public void Test04(string folderName)
    {
        _testContext.Folders.Add(new FolderDto { Id = Guid.NewGuid(), ParentId = _testContext.RootFolderId, Name = folderName });

        var home = _testContext.RenderComponent<Home>();
        home.WaitForElement("#folders .folder").MouseOver();
        home.WaitForElement("#folders .folder #go").Click();

        home.WaitForElement("#up").Click();

        home.WaitForElement($"#folders .folder .title:contains('{folderName}')").InnerHtml.MarkupMatches(folderName);
    }

    [Fact(DisplayName = "When at root it should not be possible to go to parent")]
    public void Test05()
    {
        var home = _testContext.RenderComponent<Home>();

        home.WaitForElement("#up").GetAttribute("disabled").Should().NotBeNull();
    }

    [Theory(DisplayName = "When on a child folder, it should be possible to go to parent"), AutoData]
    public void Test06(string folderName)
    {
        _testContext.Folders.Add(new FolderDto { Id = Guid.NewGuid(), ParentId = _testContext.RootFolderId, Name = folderName });

        var home = _testContext.RenderComponent<Home>();

        home.WaitForElement("#folders .folder").MouseOver();
        home.WaitForElement("#folders .folder #go").Click();

        home.WaitForElement("#up").GetAttribute("disabled").Should().BeNull();
    }

    [Fact(DisplayName = "The name of the current folder should be displayed")]
    public void Test07()
    {
        var home = _testContext.RenderComponent<Home>();

        home.WaitForElement("#current-folder-name").TextContent.MarkupMatches("Root");
    }

    [Theory(DisplayName = "Deleting a folder should remove it from list"), AutoData]
    public void Test08(string folderName)
    {
        _testContext.Folders.Add(new FolderDto { Id = Guid.NewGuid(), ParentId = _testContext.RootFolderId, Name = folderName });

        var home = _testContext.RenderComponent<Home>();

        home.WaitForElement("#folders .folder").MouseOver();
        home.WaitForElement("#folders .folder #remove").Click();

        home.WaitForElement("#folders:empty");
    }

    [Theory(DisplayName = "Creating an empty album in a folder should display it"), AutoData]
    public void Test09(string albumName)
    {
        var home = _testContext.RenderComponent<Home>();

        var newAlbumInput = home.Find("#new-album-name");
        newAlbumInput.Input(albumName);
        home.WaitForElement("#create-album").Click();

        home.WaitForElement("#albums .title").InnerHtml.MarkupMatches(albumName);
    }

    [Theory(DisplayName = "Clicking on an album should navigate to the album page"), AutoData]
    public async Task Test10(string albumName)
    {
        var home = _testContext.RenderComponent<Home>();

        var newAlbumInput = home.Find("#new-album-name");
        await newAlbumInput.InputAsync(new() { Value = albumName });
        home.WaitForElement("#create-album").Click();

        await home.WaitForElement("#albums .album .go").ClickAsync(new());
        _testContext.NavigationManager.Uri.Should().StartWith("http://localhost/album/");
    }

    #region IDisposable

    public void Dispose() => _testContext.Dispose();

    #endregion
}