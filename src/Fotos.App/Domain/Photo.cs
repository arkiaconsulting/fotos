namespace Fotos.App.Domain;

public readonly record struct Photo(PhotoId Id, string Title, ExifMetadata? Metadata = default)
{
    public Photo WithMetadata(ExifMetadata? metadata) => new(Id, Title, metadata);

    public Photo WithTitle(string title) => new(Id, title, Metadata);
}