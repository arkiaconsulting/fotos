using Fotos.Client.Components.Models;

namespace Fotos.Client.Adapters;

internal sealed record SessionData(Stack<FolderModel> FolderStack)
{
    public SessionData WithFolderStack(Stack<FolderModel> folders)
    {
        return this with { FolderStack = folders };
    }
}
