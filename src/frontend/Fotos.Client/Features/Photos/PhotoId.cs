namespace Fotos.Client.Features.Photos;

public readonly record struct PhotoId(Guid FolderId, Guid AlbumId, Guid Id);
