namespace Fotos.Client.Components.Models;

public sealed class AlbumModel
{
    public Guid Id { get; set; }
    public Guid FolderId { get; set; }
    public string Name { get; set; } = default!;
}
