namespace Fotos.App.Domain;

public readonly record struct PhotoToUpload(ReadOnlyMemory<byte> Buffer, string ContentType, string FileName);
