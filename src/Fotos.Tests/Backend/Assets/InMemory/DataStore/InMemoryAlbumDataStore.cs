using Fotos.App.Api.PhotoAlbums;
using Fotos.App.Features.PhotoAlbums;
using System.Diagnostics.CodeAnalysis;

namespace Fotos.Tests.Backend.Assets.InMemory.DataStore;

[SuppressMessage("Design", "CA1812", Justification = "Instantiated by DI")]
internal sealed class InMemoryAlbumDataStore
{
    private readonly List<Album> _source;

    public InMemoryAlbumDataStore(List<Album> source) => _source = source;

    public Task Add(Album entity)
    {
        _source.Add(entity);

        return Task.CompletedTask;
    }

    public Task Remove(AlbumId id)
    {
        _source.RemoveAll(album => album.Id == id.Id);

        return Task.CompletedTask;
    }

    public Task<Album> Get(AlbumId id) =>
        Task.FromResult(_source.Single(album => album.Id == id.Id));

    public Task<IReadOnlyCollection<Album>> GetByFolder(Guid folderId) =>
        Task.FromResult<IReadOnlyCollection<Album>>(_source.Where(album => album.FolderId == folderId).ToList());
}
