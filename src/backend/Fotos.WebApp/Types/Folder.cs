namespace Fotos.WebApp.Types;

internal readonly record struct Folder(Guid ParentId, Name Name)
{
    public static Folder Create(Guid parentId, string name)
    {
        return new Folder(parentId, Name.Create(name));
    }
}
