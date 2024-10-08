namespace Fotos.Client.Features.PhotoFolders;

internal readonly record struct Folder(Guid ParentFolderId, string Name);
