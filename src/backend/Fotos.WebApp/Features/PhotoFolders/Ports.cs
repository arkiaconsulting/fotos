namespace Fotos.WebApp.Features.PhotoFolders;

internal delegate Task AddFolderToStore(Folder folder);
internal delegate Task<IReadOnlyCollection<Folder>> GetFoldersFromStore(Guid parentId);
internal delegate Task<Folder> GetFolderFromStore(Guid parentId, Guid folderId);
internal delegate Task RemoveFolderFromStore(Guid parentId, Guid folderId);
