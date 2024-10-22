namespace Fotos.WebApp.Features.PhotoAlbums;

internal readonly record struct AlbumDto(Guid Id, Guid FolderId, string Name);
