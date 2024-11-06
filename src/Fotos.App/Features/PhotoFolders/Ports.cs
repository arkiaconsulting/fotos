using Fotos.App.Api.PhotoAlbums;
using Fotos.App.Api.PhotoFolders;
using Fotos.App.Api.Photos;
using Fotos.App.Features.PhotoAlbums;
using Fotos.App.Features.Photos;

namespace Fotos.App.Features.PhotoFolders;

internal delegate Task<IReadOnlyCollection<FolderDto>> ListFolders(Guid id);
internal delegate Task CreateFolder(Guid parentId, string folderName);
internal delegate Task<FolderDto> GetFolder(Guid parentId, Guid folderId);
internal delegate Task RemoveFolder(Guid parentId, Guid folderId);
internal delegate Task UpdateFolder(Guid parentId, Guid folderId, string folderName);
internal delegate Task CreateAlbum(Guid folderId, string albumName);
internal delegate Task<IReadOnlyCollection<AlbumDto>> ListAlbums(Guid folderId);
internal delegate Task<AlbumDto> GetAlbum(AlbumId albumId);
internal delegate Task<IReadOnlyCollection<PhotoDto>> ListPhotos(AlbumId albumId);
internal delegate Task<Guid> AddPhoto(AlbumId albumId, PhotoToUpload Binary);
internal delegate Task RemovePhoto(PhotoId photoId);
internal delegate Task<Uri> GetOriginalUri(PhotoId photoId);
internal delegate Task<Uri> GetThumbnailUri(PhotoId photoId);
internal delegate Task UpdatePhoto(PhotoId photoId, string title);
internal delegate Task<PhotoDto> GetPhoto(PhotoId photoId);