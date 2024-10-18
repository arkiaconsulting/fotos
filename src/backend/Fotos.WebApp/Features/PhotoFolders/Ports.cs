namespace Fotos.WebApp.Features.PhotoFolders;

internal delegate Task StoreNewFolder(Folder folder);
internal delegate Task<IReadOnlyCollection<Folder>> GetFolders(Guid parentId);
internal delegate Task<Folder> GetFolder(Guid parentId, Guid folderId);
internal delegate Task RemoveFolder(Guid parentId, Guid folderId);
