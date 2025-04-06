namespace Fotos.App.Domain;

internal delegate Task<IReadOnlyCollection<Album>> GetFolderAlbumsFromStore(Guid folderId);
internal delegate Task AddAlbumToStore(Album album);
internal delegate Task<Album> GetAlbumFromStore(AlbumId albumId);
internal delegate Task<int> GetAlbumPhotoCountFromStore(Guid folderId, Guid albumId);