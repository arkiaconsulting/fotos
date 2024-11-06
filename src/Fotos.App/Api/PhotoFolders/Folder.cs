using Fotos.App.Api.Shared;

namespace Fotos.App.Api.PhotoFolders;

internal readonly record struct Folder(Guid Id, Guid ParentId, Name Name)
{
    public static Folder Create(Guid id, Guid parentId, string name)
    {
        return new Folder(id, parentId, Name.Create(name));
    }
}
