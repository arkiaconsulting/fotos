namespace Fotos.Client.Features.PhotoFolders;

internal readonly record struct Folder(Guid Id, Guid ParentId, string Name);
internal readonly record struct Album(Guid Id, Guid FolderId, string Name);
internal readonly record struct Photo(Guid Id, Guid AlbumId);