namespace Fotos.App.Components.Models;

public sealed class FolderModel
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    public string Name { get; set; } = default!;

    public Folder Map() =>
        new()
        {
            Id = Id,
            ParentId = ParentId,
            Name = new Name(Name)
        };

    public static FolderModel From(Folder folder)
    {
        return new FolderModel
        {
            Id = folder.Id,
            ParentId = folder.ParentId,
            Name = folder.Name.Value
        };
    }
}
