namespace Fotos.WebApp.Types;

internal sealed record PhotoEntity(PhotoId Id, string Title, ExifMetadata? Metadata = default)
{
    public PhotoEntity WithMetadata(ExifMetadata? metadata) => new(Id, Title, metadata);
}
