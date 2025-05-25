namespace Fotos.Core;

public sealed record SessionData(Stack<Folder> FolderStack)
{
    public SessionData WithFolderStack(Stack<Folder> folders)
    {
        return this with { FolderStack = folders };
    }
}

public delegate Task AddSessionDataToStore(Guid userId, SessionData sessionData);
public delegate Task<SessionData?> GetSessionDataFromStore(Guid userId);
