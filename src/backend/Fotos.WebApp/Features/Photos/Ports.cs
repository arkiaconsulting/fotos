using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.Photos;

internal delegate Task<IReadOnlyCollection<PhotoEntity>> ListPhotos(AlbumId id);
internal delegate Task RemovePhotoData(PhotoId photoId);
internal delegate Task AddPhotoToMainStorage(PhotoId photoId, Stream photo, string contentType);
internal delegate Task<Uri> GetOriginalUri(PhotoId photoId);
internal delegate Task<Uri> GetThumbnailUri(PhotoId photoId);
internal delegate Task OnNewPhotoUploaded(PhotoId photoId);
internal delegate Task OnPhotoRemoved(PhotoId photoId);

#pragma warning disable CA1515 // Consider making public types internal (required for Azure Functions)
public delegate Task StorePhotoData(PhotoEntity photo);
public delegate Task<PhotoEntity> GetPhoto(PhotoId photoId);
public delegate Task<ExifMetadata> ExtractExifMetadata(Stream photo, string mimeType);
public delegate Task<PhotoBinary> ReadOriginalPhoto(PhotoId photoId);
public delegate Task<Stream> CreateThumbnail(PhotoBinary photo);
public delegate Task AddPhotoToThumbnailStorage(PhotoId photoId, PhotoBinary photo);
public delegate Task RemovePhotoOriginal(PhotoId photoId);
public delegate Task RemovePhotoThumbnail(PhotoId photoId);
public delegate Task OnThumbnailReady(PhotoId photoId);
#pragma warning restore CA1515 // Consider making public types internal