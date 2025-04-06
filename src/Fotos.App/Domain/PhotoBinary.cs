namespace Fotos.App.Domain;

public readonly record struct PhotoBinary(Stream Content, string MimeType);
