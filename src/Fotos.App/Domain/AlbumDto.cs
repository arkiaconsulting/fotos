namespace Fotos.App.Domain;

internal readonly record struct AlbumDto(Guid Id, Guid FolderId, string Name, int PhotoCount);
