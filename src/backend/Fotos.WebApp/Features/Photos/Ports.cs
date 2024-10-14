namespace Fotos.WebApp.Features.Photos;

internal delegate Task<IReadOnlyCollection<Photo>> ListPhotos(Guid folderId, Guid albumId);
internal delegate Task AddPhoto(Photo photo);
internal delegate Task RemovePhoto(Guid folderId, Guid albumId, Guid photoId);