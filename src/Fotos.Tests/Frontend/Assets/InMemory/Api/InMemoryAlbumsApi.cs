using Fotos.App.Api.PhotoAlbums;
using Fotos.App.Api.Shared;
using Fotos.App.Features.PhotoAlbums;
using System.Diagnostics.CodeAnalysis;

namespace Fotos.Tests.Frontend.Assets.InMemory.Api;

[SuppressMessage("Design", "CA1812", Justification = "Instantiated by DI")]
internal sealed class InMemoryAlbumsApi
{
    private readonly List<Album> _entities;

    public InMemoryAlbumsApi(List<Album> entities) => _entities = entities;

    public Task Add(Guid folderId, string name)
    {
        var album = new Album(Guid.NewGuid(), folderId, Name.Create(name));
        _entities.Add(album);

        return Task.FromResult(album);
    }

    public Task<IReadOnlyCollection<AlbumDto>> List(Guid folderId)
    {
        var result = _entities
            .Where(a => a.FolderId == folderId)
            .Select(a => new AlbumDto(a.Id, a.FolderId, a.Name.Value))
            .ToList();

        return Task.FromResult<IReadOnlyCollection<AlbumDto>>(result);
    }

    public Task<AlbumDto> Get(AlbumId albumId)
    {
        var album = _entities.Single(a => a.Id == albumId.Id);

        return Task.FromResult(new AlbumDto(album.Id, album.FolderId, album.Name.Value));
    }
}
