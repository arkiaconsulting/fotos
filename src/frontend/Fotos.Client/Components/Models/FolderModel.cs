namespace Fotos.Client.Components.Models;

internal sealed class FolderModel
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    public string Name { get; set; } = default!;
}
