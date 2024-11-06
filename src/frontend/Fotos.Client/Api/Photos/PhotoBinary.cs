namespace Fotos.Client.Api.Photos;

public readonly record struct PhotoBinary(Stream Content, string MimeType);
