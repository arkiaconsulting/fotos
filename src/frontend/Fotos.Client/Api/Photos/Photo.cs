using Fotos.Client.Features.Photos;

namespace Fotos.Client.Api.Photos;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public readonly record struct Photo(PhotoId Id, string Title, ExifMetadata? Metadata = default)
{
    public Photo WithMetadata(ExifMetadata? metadata) => new(Id, Title, metadata);

    public Photo WithTitle(string title) => new(Id, title, Metadata);
}