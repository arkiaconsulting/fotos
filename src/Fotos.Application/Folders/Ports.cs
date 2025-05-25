namespace Fotos.Application.Folders;

public delegate Task AddFolderToStore(Folder folder);
public delegate Task<IReadOnlyCollection<Folder>> GetFoldersFromStore(Guid parentId);
public delegate Task<Folder> GetFolderFromStore(Guid parentId, Guid folderId);
public delegate Task RemoveFolderFromStore(Guid parentId, Guid folderId);
public delegate Task UpdateFolderInStore(Guid parentId, Guid folderId, Name name);
