namespace Fotos.Client.Adapters;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public readonly record struct FolderDto(Guid Id, Guid ParentId, string Name);

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public readonly record struct AlbumDto(Guid Id, Guid FolderId, string Name);

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public readonly record struct PhotoDto(Guid Id, Guid FolderId, Guid AlbumId, string Title);
