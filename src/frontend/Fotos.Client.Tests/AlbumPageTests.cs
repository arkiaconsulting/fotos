using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.Client.Components.Pages;
using Fotos.Client.Features.PhotoFolders;
using Fotos.Client.Tests.Assets;
using Microsoft.AspNetCore.Components.Forms;

namespace Fotos.Client.Tests;

[Trait("Category", "Unit")]
public sealed class AlbumPageTests : IDisposable
{
    private readonly FotosTestContext _testContext;

    public AlbumPageTests() => _testContext = new FotosTestContext();

    [Theory(DisplayName = "The album page should display the name of the album"), AutoData]
    public void Test01(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new Album { Id = albumId, FolderId = folderId, Name = albumName });
        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        cut.WaitForElement("#album-name").TextContent.Should().Be(albumName);
    }

    [Theory(DisplayName = "The album page should display the attached photos"), AutoData]
    public void Test02(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new Album { Id = albumId, FolderId = folderId, Name = albumName });
        _testContext.Photos.Add(new Photo { Id = Guid.NewGuid(), AlbumId = albumId });

        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        cut.WaitForElements("#thumbnails img").Should().HaveCount(1);
    }

    [Theory(DisplayName = "The album page should allow the uploading of an photo"), AutoData]
    public void Test03(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new Album { Id = albumId, FolderId = folderId, Name = albumName });
        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        cut.WaitForElement("#album input[type=file]");
    }

    [Theory(DisplayName = "The album page should display a photo that was just uploaded"), AutoData]
    public void Test04(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new Album { Id = albumId, FolderId = folderId, Name = albumName });
        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));
        cut.WaitForElement("#album input[type=file]");
        cut.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromBinary([0x00]));

        cut.WaitForElements("#thumbnails .thumbnail img").Should().HaveCount(1);
    }

    [Theory(DisplayName = "Removing a photo from an album should remove it from list"), AutoData]
    public void Test05(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new Album { Id = albumId, FolderId = folderId, Name = albumName });
        _testContext.Photos.Add(new Photo { Id = Guid.NewGuid(), AlbumId = albumId });

        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));
        cut.WaitForElement("#thumbnails .thumbnail img");

        var removeButton = cut.Find("#thumbnails .thumbnail button.remove");
        removeButton.Click();

        cut.WaitForAssertion(() => cut.Find("#thumbnails").InnerHtml.MarkupMatches(""));
    }

    [Theory(DisplayName = "Viewing a photo should display it"), AutoData]
    public void Test06(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new Album { Id = albumId, FolderId = folderId, Name = albumName });
        _testContext.Photos.Add(new Photo { Id = Guid.NewGuid(), AlbumId = albumId });

        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        var viewButton = cut.WaitForElement("#thumbnails .thumbnail button.view");
        viewButton.Click();

        cut.WaitForAssertion(() => cut.Find("#photo img"));
    }

    [Theory(DisplayName = "Dismissing a displayed photo should hide it"), AutoData]
    public void Test07(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new Album { Id = albumId, FolderId = folderId, Name = albumName });
        _testContext.Photos.Add(new Photo { Id = Guid.NewGuid(), AlbumId = albumId });

        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));
        var viewButton = cut.WaitForElement("#thumbnails .thumbnail button.view");
        viewButton.Click();

        var closeButton = cut.Find("#photo button.dismiss");
        closeButton.Click();

        cut.WaitForAssertion(() => cut.FindAll("#photo").Should().BeEmpty());
    }

    #region IDisposable

    public void Dispose() => _testContext.Dispose();

    #endregion
}