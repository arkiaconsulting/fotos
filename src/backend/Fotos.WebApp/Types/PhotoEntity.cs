namespace Fotos.WebApp.Types;

internal sealed record PhotoEntity(PhotoId Id, ExifMetadata? Metadata = default)
{
    public PhotoEntity WithMetadata(ExifMetadata? metadata) => new(Id, metadata);
}
