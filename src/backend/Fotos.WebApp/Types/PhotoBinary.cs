namespace Fotos.WebApp.Types;

internal readonly record struct PhotoBinary(Stream Content, string MimeType);
