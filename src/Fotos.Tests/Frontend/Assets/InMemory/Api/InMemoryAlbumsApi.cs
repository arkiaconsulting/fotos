using Fotos.App.Api.PhotoAlbums;
using Fotos.App.Api.Photos;
using Fotos.App.Api.Shared;
using Fotos.App.Features.PhotoAlbums;
using System.Diagnostics.CodeAnalysis;

namespace Fotos.Tests.Frontend.Assets.InMemory.Api;

[SuppressMessage("Design", "CA1812", Justification = "Instantiated by DI")]
internal sealed class InMemoryAlbumsApi
{
    private readonly List<Album> _albums;
    private readonly List<Photo> _photos;

    public InMemoryAlbumsApi(
        List<Album> albums,
        List<Photo> photos)
    {
        _albums = albums;
        _photos = photos;
    }

    public Task Add(Guid folderId, string name)
    {
        var album = new Album(Guid.NewGuid(), folderId, Name.Create(name));
        _albums.Add(album);

        return Task.FromResult(album);
    }

    public Task<IReadOnlyCollection<AlbumDto>> List(Guid folderId)
    {
        var result = _albums
            .Where(a => a.FolderId == folderId)
            .Select(a => new AlbumDto(a.Id, a.FolderId, a.Name.Value, _photos.Count(p => p.Id.AlbumId == a.Id)))
            .ToList();

        return Task.FromResult<IReadOnlyCollection<AlbumDto>>(result);
    }

    public Task<AlbumDto> Get(AlbumId albumId)
    {
        var album = _albums.Single(a => a.Id == albumId.Id);

        return Task.FromResult(new AlbumDto(album.Id, album.FolderId, album.Name.Value, _photos.Count(p => p.Id.AlbumId == album.Id)));
    }
}
