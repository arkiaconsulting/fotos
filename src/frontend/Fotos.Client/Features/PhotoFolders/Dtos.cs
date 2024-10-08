namespace Fotos.Client.Features.PhotoFolders;

internal readonly record struct Folder(Guid ParentId, string Name);
