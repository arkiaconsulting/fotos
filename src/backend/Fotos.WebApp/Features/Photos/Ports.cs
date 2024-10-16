using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.Photos;

internal delegate Task<IReadOnlyCollection<PhotoEntity>> ListPhotos(AlbumId id);
internal delegate Task RemovePhoto(PhotoId photoId);
internal delegate Task StorePhotoData(PhotoEntity photo);
internal delegate Task AddPhotoToMainStorage(PhotoId photoId, Stream photo, string contentType);
internal delegate Task OnNewPhotoUploaded(PhotoId photoId);
internal delegate Task<ExifMetadata> ExtractExifMetadata(Stream photo);
internal delegate Task<PhotoEntity> GetPhoto(PhotoId photoId);
internal delegate Task<Uri> GetOriginalUri(PhotoId photoId);

#pragma warning disable CA1515 // Consider making public types internal (required for Azure Functions)
public delegate Task<PhotoBinary> ReadOriginalPhoto(PhotoId photoId);
public delegate Task<Stream> CreateThumbnail(PhotoBinary photo);
public delegate Task AddPhotoToThumbnailStorage(PhotoId photoId, PhotoBinary photo);
#pragma warning restore CA1515 // Consider making public types internal