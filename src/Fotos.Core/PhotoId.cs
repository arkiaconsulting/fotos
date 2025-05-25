namespace Fotos.Core;

public readonly record struct PhotoId(Guid FolderId, Guid AlbumId, Guid Id);
