namespace Fotos.Client.Components.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed class AlbumModel
{
    public Guid Id { get; set; }
    public Guid FolderId { get; set; }
    public string Name { get; set; } = default!;
}
