namespace Fotos.Client.Features.PhotoFolders;

internal readonly record struct Folder(Guid Id, Guid ParentFolderId, string Name);
