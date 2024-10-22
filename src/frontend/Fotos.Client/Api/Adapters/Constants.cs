using System.Text.Json;

namespace Fotos.Client.Api.Adapters;

internal static class Constants
{
    public const string BlobServiceClientName = "MainStorage";
    public const string ServiceBusClientName = "ServiceBus";

    public static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    public const int ThumbnailMaxWidth = 200;
    public const int ThumbnailMaxHeight = 200;
}
