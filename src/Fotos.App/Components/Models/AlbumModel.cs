namespace Fotos.App.Components.Models;

public sealed class AlbumModel
{
    public Guid Id { get; set; }
    public Guid FolderId { get; set; }
    public string Name { get; set; } = default!;
    public int PhotoCount { get; set; }
}
