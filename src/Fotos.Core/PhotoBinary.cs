namespace Fotos.Core;

public readonly record struct PhotoBinary(Stream Content, string MimeType);
