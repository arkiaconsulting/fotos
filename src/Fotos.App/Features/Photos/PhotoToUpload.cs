namespace Fotos.App.Features.Photos;

public readonly record struct PhotoToUpload(ReadOnlyMemory<byte> Buffer, string ContentType, string FileName);
