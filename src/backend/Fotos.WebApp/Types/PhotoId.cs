namespace Fotos.WebApp.Types;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public readonly record struct PhotoId(Guid FolderId, Guid AlbumId, Guid Id);
