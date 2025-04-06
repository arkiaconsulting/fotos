using Fotos.App.Adapters;
using Fotos.App.Domain;

namespace Fotos.App.Application.Photos;

// Store
internal delegate Task<IReadOnlyCollection<Photo>> ListPhotosFromStore(AlbumId id);
internal delegate Task RemovePhotoFromStore(PhotoId photoId);
public delegate Task AddPhotoToStore(Photo photo);
public delegate Task<Photo> GetPhotoFromStore(PhotoId photoId);
internal delegate Task AddSessionDataToStore(Guid userId, SessionData sessionData);
internal delegate Task<SessionData?> GetSessionDataFromStore(Guid userId);

// Storage
public delegate Task AddPhotoToMainStorage(PhotoId photoId, Stream photo, string contentType);
internal delegate Task<Uri> GetOriginalStorageUri(PhotoId photoId);
internal delegate Task<Uri> GetThumbnailStorageUri(PhotoId photoId);
public delegate Task<PhotoBinary> ReadOriginalPhotoFromStorage(PhotoId photoId);
public delegate Task AddPhotoToThumbnailStorage(PhotoId photoId, PhotoBinary photo);
public delegate Task RemovePhotoOriginalFromStorage(PhotoId photoId);
public delegate Task RemovePhotoThumbnailFromStorage(PhotoId photoId);

// Messaging
internal delegate Task OnNewPhotoUploaded(PhotoId photoId);
internal delegate Task OnPhotoRemoved(PhotoId photoId);

// Various
public delegate Task<Stream> CreateThumbnail(PhotoBinary photo);
public delegate Task<ExifMetadata> ExtractExifMetadata(Stream photo, string mimeType);
