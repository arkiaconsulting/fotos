using Fotos.App.Features.Photos;

namespace Fotos.App.Api.Photos;

public readonly record struct Photo(PhotoId Id, string Title, ExifMetadata? Metadata = default)
{
    public Photo WithMetadata(ExifMetadata? metadata) => new(Id, Title, metadata);

    public Photo WithTitle(string title) => new(Id, title, Metadata);
}