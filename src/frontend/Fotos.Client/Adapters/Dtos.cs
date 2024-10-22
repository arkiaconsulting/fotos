namespace Fotos.Client.Adapters;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public readonly record struct Folder(Guid Id, Guid ParentId, string Name);

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public readonly record struct Album(Guid Id, Guid FolderId, string Name);

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public readonly record struct Photo(Guid Id, Guid FolderId, Guid AlbumId, string Title, Uri ThumbnailUri);

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public readonly record struct PhotoBinary(ReadOnlyMemory<byte> Buffer, string ContentType, string FileName);
