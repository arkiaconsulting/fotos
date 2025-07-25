﻿using Fotos.Core;
using System.Diagnostics.CodeAnalysis;

namespace Fotos.Tests.Assets.InMemory.DataStore;

[SuppressMessage("Design", "CA1812", Justification = "Instantiated by DI")]
internal sealed class InMemoryPhotoDataStore
{
    private readonly List<Photo> _source;

    public InMemoryPhotoDataStore(List<Photo> source) => _source = source;

    public Task Add(Photo entity)
    {
        var existing = _source.SingleOrDefault(photo => photo.Id == entity.Id);
        if (existing != new Photo())
        {
            _source.Remove(existing);
        }

        _source.Add(entity);

        return Task.CompletedTask;
    }

    public Task Remove(PhotoId id)
    {
        _source.RemoveAll(photo => photo.Id == id);

        return Task.CompletedTask;
    }

    public Task<Photo> Get(PhotoId id) =>
        Task.FromResult(_source.Single(photo => photo.Id == id));

    public Task<IReadOnlyCollection<Photo>> GetByAlbum(AlbumId albumId) =>
        Task.FromResult<IReadOnlyCollection<Photo>>([.. _source.Where(photo => photo.Id.AlbumId == albumId.Id)]);

    public Task<int> CountPhotos(Guid _, Guid albumId) =>
        Task.FromResult(_source.Count(photo => photo.Id.AlbumId == albumId));
}
