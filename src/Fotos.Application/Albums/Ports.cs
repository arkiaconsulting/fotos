namespace Fotos.Application.Albums;

public delegate Task<IReadOnlyCollection<Album>> GetFolderAlbumsFromStore(Guid folderId);
public delegate Task AddAlbumToStore(Album album);
public delegate Task<Album> GetAlbumFromStore(AlbumId albumId);
public delegate Task<int> GetAlbumPhotoCountFromStore(Guid folderId, Guid albumId);