using Fotos.App.Api.Shared;

namespace Fotos.App.Api.PhotoFolders;

internal delegate Task AddFolderToStore(Folder folder);
internal delegate Task<IReadOnlyCollection<Folder>> GetFoldersFromStore(Guid parentId);
internal delegate Task<Folder> GetFolderFromStore(Guid parentId, Guid folderId);
internal delegate Task RemoveFolderFromStore(Guid parentId, Guid folderId);
internal delegate Task UpdateFolderInStore(Guid parentId, Guid folderId, Name name);
