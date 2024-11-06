namespace Fotos.Client.Features.Photos;

public readonly record struct PhotoToUpload(ReadOnlyMemory<byte> Buffer, string ContentType, string FileName);
