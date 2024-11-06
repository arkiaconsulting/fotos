using Fotos.App.Api.Photos;
using Fotos.App.Features.PhotoAlbums;
using Fotos.App.Features.Photos;
using System.Diagnostics.CodeAnalysis;

namespace Fotos.Tests.Frontend.Assets.InMemory.Api;

[SuppressMessage("Design", "CA1812", Justification = "Instantiated by DI")]
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
internal sealed class InMemoryPhotosApi
{
    private readonly List<Photo> _entities;

    public InMemoryPhotosApi(List<Photo> entities) => _entities = entities;

    public Task<Guid> Add(AlbumId albumId, PhotoToUpload _)
    {
        var photoId = Guid.NewGuid();
        var photo = new Photo(new(albumId.FolderId, albumId.Id, photoId), "title", new());
        _entities.Add(photo);

        return Task.FromResult(photoId);
    }

    public Task<IReadOnlyCollection<PhotoDto>> List(AlbumId albumId)
    {
        var result = _entities
            .Where(p => p.Id.AlbumId == albumId.Id)
            .Select(p => new PhotoDto(p.Id.Id, p.Id.FolderId, p.Id.AlbumId, p.Title, p.Metadata ?? new()))
            .ToList();

        return Task.FromResult<IReadOnlyCollection<PhotoDto>>(result);
    }

    public Task<PhotoDto> Get(PhotoId photoId)
    {
        var photo = _entities.Single(p => p.Id == photoId);

        return Task.FromResult(new PhotoDto(photo.Id.Id, photo.Id.FolderId, photo.Id.AlbumId, photo.Title, photo.Metadata ?? new()));
    }

    public Task Remove(PhotoId photoId)
    {
        var photo = _entities.Single(p => p.Id == photoId);
        _entities.Remove(photo);

        return Task.CompletedTask;
    }

    public Task Update(PhotoId photoId, string title)
    {
        var photo = _entities.Single(p => p.Id == photoId);
        _entities.Remove(photo);
        _entities.Add(new Photo(photo.Id, title, photo.Metadata ?? new()));

        return Task.CompletedTask;
    }

    public Task<Uri> GetOriginalUri(PhotoId _)
    {
        return Task.FromResult(new Uri("/", UriKind.Relative));
    }

    public Task<Uri> GetThumbnailUri(PhotoId _)
    {
        return Task.FromResult(new Uri("/", UriKind.Relative));
    }
}
