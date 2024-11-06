using Fotos.App.Components.Models;

namespace Fotos.App.Adapters;

internal sealed record SessionData(Stack<FolderModel> FolderStack)
{
    public SessionData WithFolderStack(Stack<FolderModel> folders)
    {
        return this with { FolderStack = folders };
    }
}
