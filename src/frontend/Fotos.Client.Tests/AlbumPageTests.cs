using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.Client.Adapters;
using Fotos.Client.Components.Pages;
using Fotos.Client.Tests.Assets;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Mime;

namespace Fotos.Client.Tests;

[Trait("Category", "Unit")]
public sealed class AlbumPageTests : IDisposable
{
    private readonly FotosTestContext _testContext;

    public AlbumPageTests() => _testContext = new FotosTestContext();

    [Theory(DisplayName = "The album page should display the name of the album"), AutoData]
    public void Test01(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new AlbumDto { Id = albumId, FolderId = folderId, Name = albumName });
        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        cut.WaitForElement("#album-name").TextContent.Should().Be(albumName);
    }

    [Theory(DisplayName = "The album page should display the attached photos"), AutoData]
    public void Test02(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new AlbumDto { Id = albumId, FolderId = folderId, Name = albumName });
        _testContext.Photos.Add(new PhotoDto { Id = Guid.NewGuid(), AlbumId = albumId });

        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        cut.WaitForElements("#thumbnails img").Should().HaveCount(1);
    }

    [Theory(DisplayName = "The album page should allow the uploading of an photo"), AutoData]
    public void Test03(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new AlbumDto { Id = albumId, FolderId = folderId, Name = albumName });
        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        cut.WaitForElement("#album input[type=file]");
    }

    [Theory(DisplayName = "The album page should display a photo that was just uploaded"), AutoData]
    public void Test04(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new AlbumDto { Id = albumId, FolderId = folderId, Name = albumName });
        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        cut.WaitForElement("#album input[type=file]");
        cut.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromBinary([0x00], contentType: MediaTypeNames.Image.Jpeg));

        cut.WaitForElements("#thumbnails .thumbnail img").Should().HaveCount(1);
    }

    [Theory(DisplayName = "Removing a photo from an album should remove it from list"), AutoData]
    public async Task Test05(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new AlbumDto { Id = albumId, FolderId = folderId, Name = albumName });
        _testContext.Photos.Add(new PhotoDto { Id = Guid.NewGuid(), AlbumId = albumId });

        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        await cut.WaitForElement("#thumbnails .thumbnail button.view").ClickAsync(new());

        await _testContext.Popover.WaitForElement("#details .remove").ClickAsync(new());

        cut.WaitForAssertion(() => cut.Find("#thumbnails").InnerHtml.MarkupMatches(""));
    }

    [Theory(DisplayName = "Viewing a photo should display it"), AutoData]
    public void Test06(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new AlbumDto { Id = albumId, FolderId = folderId, Name = albumName });
        _testContext.Photos.Add(new PhotoDto { Id = Guid.NewGuid(), AlbumId = albumId });

        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        cut.WaitForElement("#thumbnails .thumbnail button.view").DoubleClick();

        cut.WaitForAssertion(() => cut.Find("#photo img"));
    }

    [Theory(DisplayName = "Dismissing a displayed photo should hide it"), AutoData]
    public void Test07(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new AlbumDto { Id = albumId, FolderId = folderId, Name = albumName });
        _testContext.Photos.Add(new PhotoDto { Id = Guid.NewGuid(), AlbumId = albumId });

        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        cut.WaitForElement("#thumbnails .thumbnail button.view").DoubleClick();

        cut.WaitForElement("#photo img").Click();

        cut.WaitForAssertion(() => cut.FindAll("#photo").Should().BeEmpty());
    }

    [Theory(DisplayName = "Uploading a photo that is larger than 20MB should not be allowed"), AutoData]
    public void Test08(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new AlbumDto { Id = albumId, FolderId = folderId, Name = albumName });
        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));
        cut.WaitForElement("#album input[type=file]");
        cut.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromBinary(new byte[21L * 1024L * 1024L]));

        _testContext.SnackBar.WaitForElement("#alert").TextContent.Should().Be("The file is too large.");
    }

    [Theory(DisplayName = "Uploading a photo that has not an allowed type should not be allowed"), AutoData]
    public void Test09(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new AlbumDto { Id = albumId, FolderId = folderId, Name = albumName });
        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        cut.WaitForElement("#album input[type=file]");
        cut.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromBinary([0x00], contentType: MediaTypeNames.Application.Pdf));

        _testContext.SnackBar.WaitForElement("#alert").TextContent.Should().Be("Only photos can be uploaded.");
    }

    [Theory(DisplayName = "The name of a photo that was just uploaded should be its original file name"), AutoData]
    public void Test10(Guid folderId, Guid albumId, string albumName, string fileName)
    {
        _testContext.Albums.Add(new AlbumDto { Id = albumId, FolderId = folderId, Name = albumName });
        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        cut.WaitForElement("#album input[type=file]");
        cut.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromBinary([0x00], fileName: fileName, contentType: MediaTypeNames.Image.Jpeg));

        cut.WaitForAssertion(() => cut.Find($"#thumbnails .thumbnail img[alt='{fileName}']"));
    }

    [Theory(DisplayName = "When on the albums page, should be able to go back to the parent folder view"), AutoData]
    public async Task Test11(Guid folderId, Guid albumId, string albumName)
    {
        _testContext.Albums.Add(new AlbumDto { Id = albumId, FolderId = folderId, Name = albumName });
        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        await cut.WaitForElement("#go-parent-folder").ClickAsync(new());

        _testContext.Services.GetRequiredService<NavigationManager>().Uri.Should().Be($"http://localhost/");
    }

    [Theory(DisplayName = "Clicking on a photo should display its title"), AutoData]
    public async Task Test12(Guid folderId, Guid albumId, string albumName, string title)
    {
        _testContext.Albums.Add(new AlbumDto { Id = albumId, FolderId = folderId, Name = albumName });
        _testContext.Photos.Add(new PhotoDto { Id = Guid.NewGuid(), AlbumId = albumId, Title = title });
        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        await cut.WaitForElement("#thumbnails .thumbnail button.view").ClickAsync(new());

        _testContext.Popover.WaitForElement("#title").GetAttribute("value").Should().Be(title);
    }

    [Theory(DisplayName = "Modifying the title of a photo should update the title"), AutoData]
    public async Task Test13(Guid folderId, Guid albumId, string albumName, string title, string newTitle)
    {
        _testContext.Albums.Add(new AlbumDto { Id = albumId, FolderId = folderId, Name = albumName });
        var photoId = Guid.NewGuid();
        _testContext.Photos.Add(new PhotoDto { Id = photoId, AlbumId = albumId, Title = title });
        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));
        await cut.WaitForElement("#thumbnails .thumbnail button.view").ClickAsync(new());

        _testContext.Popover.WaitForElement("#details #title").Change(newTitle);

        cut.WaitForAssertion(() => _testContext.Photos.Single(p => p.Id == photoId).Title.Should().Be(newTitle));
    }

    #region IDisposable

    public void Dispose() => _testContext.Dispose();

    #endregion
}