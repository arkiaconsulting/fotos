namespace Fotos.WebApp.Types;

internal sealed record PhotoEntity(Guid FolderId, Guid AlbumId, Guid Id, Uri Uri, ExifMetadata? Metadata = default)
{
    public PhotoEntity WithMetadata(ExifMetadata? metadata) => new(FolderId, AlbumId, Id, Uri, metadata);
}
