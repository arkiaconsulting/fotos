using System.Text.Json;

namespace Fotos.App.Adapters;

internal static class Constants
{
    public static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    public const int ThumbnailMaxWidth = 200;
    public const int ThumbnailMaxHeight = 200;
}
