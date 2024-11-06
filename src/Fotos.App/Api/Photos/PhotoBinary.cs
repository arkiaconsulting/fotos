namespace Fotos.App.Api.Photos;

public readonly record struct PhotoBinary(Stream Content, string MimeType);
