using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.Photos;

internal delegate Task<IReadOnlyCollection<PhotoEntity>> ListPhotos(AlbumId id);
internal delegate Task RemovePhoto(PhotoId photoId);
internal delegate Task StorePhotoData(PhotoEntity photo);
internal delegate Task AddPhotoToMainStorage(Guid Id, Stream photo);
internal delegate Task OnNewPhotoUploaded(Guid photoId);
internal delegate Task<Stream> ReadOriginalPhoto(Guid photoId);
internal delegate Task<ExifMetadata> ExtractExifMetadata(Stream photo);
internal delegate Task<PhotoEntity> GetPhoto(PhotoId photoId);