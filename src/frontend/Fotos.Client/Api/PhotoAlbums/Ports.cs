using Fotos.Client.Features.PhotoAlbums;

namespace Fotos.Client.Api.PhotoAlbums;

internal delegate Task<IReadOnlyCollection<Album>> GetFolderAlbumsFromStore(Guid folderId);
internal delegate Task AddAlbumToStore(Album album);
internal delegate Task<Album> GetAlbumFromStore(AlbumId albumId);