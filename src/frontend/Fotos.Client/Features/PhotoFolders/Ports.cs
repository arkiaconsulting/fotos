namespace Fotos.Client.Features.PhotoFolders;

internal delegate Task<IReadOnlyCollection<Folder>> ListFolders(Guid parentId);
internal delegate Task CreateFolder(Guid parentId, string folderName);
internal delegate Task<Folder> GetFolder(Guid folderId);
internal delegate Task RemoveFolder(Guid folderId);
internal delegate Task CreateAlbum(Guid folderId, string albumName);
internal delegate Task<IReadOnlyCollection<Album>> ListAlbums(Guid folderId);
internal delegate Task<Album> GetAlbum(Guid folderId, Guid albumId);
internal delegate Task<IReadOnlyCollection<Photo>> ListPhotos(Guid folderId, Guid albumId);
internal delegate Task<Guid> AddPhoto(Guid folderId, Guid albumId, PhotoBinary Binary);
internal delegate Task RemovePhoto(Guid folderId, Guid albumId, Guid id);
internal delegate Task<Uri> GetOriginalUri(Guid folderId, Guid albumId, Guid id);
internal delegate Task<Uri> GetThumbnailUri(Guid folderId, Guid albumId, Guid id);