namespace Fotos.Client.Features.PhotoFolders;

internal delegate Task<IReadOnlyCollection<Folder>> ListFolders(Guid parentFolderId);
internal delegate Task<Guid> GetRootFolderId();
