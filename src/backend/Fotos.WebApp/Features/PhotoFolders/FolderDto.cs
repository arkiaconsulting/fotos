namespace Fotos.WebApp.Features.PhotoFolders;

internal readonly record struct FolderDto(Guid Id, Guid ParentId, string Name);