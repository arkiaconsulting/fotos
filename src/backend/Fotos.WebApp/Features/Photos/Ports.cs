using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.Photos;

internal delegate Task<IReadOnlyCollection<PhotoEntity>> ListPhotos(AlbumId id);
internal delegate Task RemovePhoto(PhotoId photoId);
internal delegate Task StorePhotoData(PhotoEntity photo);
internal delegate Task AddPhotoToMainStorage(PhotoId photoId, Stream photo, string contentType);
internal delegate Task OnNewPhotoUploaded(PhotoId photoId);
internal delegate Task<PhotoBinary> ReadOriginalPhoto(PhotoId photoId);
internal delegate Task<ExifMetadata> ExtractExifMetadata(Stream photo);
internal delegate Task<PhotoEntity> GetPhoto(PhotoId photoId);
internal delegate Task<Stream> CreateThumbnail(PhotoBinary photo);
internal delegate Task AddPhotoToThumbnailStorage(PhotoId photoId, PhotoBinary photo);
internal delegate Task<Uri> GetOriginalUri(PhotoId photoId);