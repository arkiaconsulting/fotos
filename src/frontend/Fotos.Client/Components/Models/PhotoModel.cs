namespace Fotos.Client.Components.Models;

internal sealed record PhotoModel(Guid FolderId, Guid AlbumId, Guid Id, Uri OriginalUri);
