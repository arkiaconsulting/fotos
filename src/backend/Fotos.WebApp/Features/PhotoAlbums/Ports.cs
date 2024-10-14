using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.PhotoAlbums;

internal delegate Task<IReadOnlyCollection<Album>> GetFolderAlbums(Guid folderId);
internal delegate Task AddAlbum(Album album);
internal delegate Task<Album> GetAlbum(AlbumId albumId);