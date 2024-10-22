namespace Fotos.Client.Api.PhotoFolders;

internal readonly record struct FolderDto(Guid Id, Guid ParentId, string Name);