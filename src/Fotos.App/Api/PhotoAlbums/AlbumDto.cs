namespace Fotos.App.Api.PhotoAlbums;

internal readonly record struct AlbumDto(Guid Id, Guid FolderId, string Name, int PhotoCount);
