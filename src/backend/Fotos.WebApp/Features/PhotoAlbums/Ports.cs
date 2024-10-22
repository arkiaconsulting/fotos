namespace Fotos.WebApp.Features.PhotoAlbums;

internal delegate Task<IReadOnlyCollection<Album>> GetFolderAlbumsFromStore(Guid folderId);
internal delegate Task AddAlbumToStore(Album album);
internal delegate Task<Album> GetAlbumFromStore(AlbumId albumId);