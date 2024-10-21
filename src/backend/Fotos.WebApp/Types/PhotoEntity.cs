namespace Fotos.WebApp.Types;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed record PhotoEntity(PhotoId Id, string Title, ExifMetadata? Metadata = default)
{
    public PhotoEntity WithMetadata(ExifMetadata? metadata) => new(Id, Title, metadata);

    public PhotoEntity WithTitle(string title) => new(Id, title, Metadata);
}
