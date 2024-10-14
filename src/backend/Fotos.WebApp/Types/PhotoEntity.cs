namespace Fotos.WebApp.Types;

internal sealed record PhotoEntity(PhotoId Id, Uri Uri, ExifMetadata? Metadata = default)
{
    public PhotoEntity WithMetadata(ExifMetadata? metadata) => new(Id, Uri, metadata);
}
