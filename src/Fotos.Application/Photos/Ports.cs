namespace Fotos.Application.Photos;

// Store
public delegate Task<IReadOnlyCollection<Photo>> ListPhotosFromStore(AlbumId id);
public delegate Task RemovePhotoFromStore(PhotoId photoId);
public delegate Task AddPhotoToStore(Photo photo);
public delegate Task<Photo> GetPhotoFromStore(PhotoId photoId);

// Storage
public delegate Task AddPhotoToMainStorage(PhotoId photoId, Stream photo, string contentType);
public delegate Task<Uri> GetOriginalStorageUri(PhotoId photoId);
public delegate Task<Uri> GetThumbnailStorageUri(PhotoId photoId);
public delegate Task<PhotoBinary> ReadOriginalPhotoFromStorage(PhotoId photoId);
public delegate Task AddPhotoToThumbnailStorage(PhotoId photoId, PhotoBinary photo);
public delegate Task RemovePhotoOriginalFromStorage(PhotoId photoId);
public delegate Task RemovePhotoThumbnailFromStorage(PhotoId photoId);

// Messaging
public delegate Task OnNewPhotoUploaded(PhotoId photoId);
public delegate Task OnPhotoRemoved(PhotoId photoId);

// Various
public delegate Task<Stream> CreateThumbnail(PhotoBinary photo);
public delegate Task<ExifMetadata> ExtractExifMetadata(Stream photo, string mimeType);
