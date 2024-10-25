namespace Fotos.Client.Adapters;

internal readonly record struct FolderDto(Guid Id, Guid ParentId, string Name);

internal readonly record struct AlbumDto(Guid Id, Guid FolderId, string Name);

internal readonly record struct PhotoDto(Guid Id, Guid FolderId, Guid AlbumId, string Title);
