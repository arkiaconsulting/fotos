namespace Fotos.App.Domain;

public readonly record struct PhotoId(Guid FolderId, Guid AlbumId, Guid Id);
