using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.PhotoFolders;

internal delegate Task StoreNewFolder(Folder folder);
internal delegate Task<IReadOnlyCollection<Folder>> GetFolders(Guid folderId);
internal delegate Task<Folder> GetFolder(Guid folderId);
internal delegate Task RemoveFolder(Guid folderId);
