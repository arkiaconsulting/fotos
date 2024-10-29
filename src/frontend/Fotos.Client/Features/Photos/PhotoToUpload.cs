namespace Fotos.Client.Features.Photos;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public readonly record struct PhotoToUpload(ReadOnlyMemory<byte> Buffer, string ContentType, string FileName);
