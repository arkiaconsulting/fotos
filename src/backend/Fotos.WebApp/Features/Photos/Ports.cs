namespace Fotos.WebApp.Features.Photos;

internal delegate Task<IReadOnlyCollection<Photo>> ListPhotos(Guid folderId, Guid albumId);
internal delegate Task RemovePhoto(Guid folderId, Guid albumId, Guid photoId);
internal delegate Task StorePhotoData(Photo photo);
internal delegate Task AddPhotoToMainStorage(Guid Id, Stream photo);
internal delegate Task OnNewPhotoUploaded(Guid photoId);