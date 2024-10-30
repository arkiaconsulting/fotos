using Fotos.Client.Api.PhotoAlbums;
using Fotos.Client.Api.PhotoFolders;
using Fotos.Client.Api.Photos;
using Fotos.Client.Features.PhotoAlbums;
using Fotos.Client.Features.Photos;

namespace Fotos.Client.Features.PhotoFolders;

internal delegate Task<IReadOnlyCollection<FolderDto>> ListFolders(Guid parentId);
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