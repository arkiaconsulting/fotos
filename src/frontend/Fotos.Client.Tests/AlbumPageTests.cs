using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.Client.Components.Pages;
using Fotos.Client.Features.PhotoFolders;
using Fotos.Client.Tests.Assets;

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
    public void Test02(Guid folderId, Guid albumId, string albumName, Uri photoUrl)
    {
        _testContext.Albums.Add(new Album { Id = albumId, FolderId = folderId, Name = albumName });
        _testContext.Photos.Add(new Photo { Id = Guid.NewGuid(), AlbumId = albumId, Url = photoUrl });

        var cut = _testContext.RenderComponent<AnAlbum>(parameters =>
        parameters.Add(p => p.FolderId, folderId).Add(p => p.AlbumId, albumId));

        cut.WaitForElements("#photos img").Should().HaveCount(1);
    }

    #region IDisposable

    public void Dispose() => _testContext.Dispose();

    #endregion
}