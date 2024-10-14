namespace Fotos.WebApp.Types;

internal readonly record struct PhotoId(Guid FolderId, Guid AlbumId, Guid Id);
