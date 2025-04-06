namespace Fotos.App.Domain;

internal static class Constants
{
    public const long MaxPhotoSize = 20L * 1024L * 1024L;
    public static string[] AllowedPhotoContentTypes = ["image/jpeg", "image/png", "image/jpg"];
}
