﻿using System.Text.Json;

namespace Fotos.WebApp.Features.Photos.Adapters;

internal static class Constants
{
    public const string BlobServiceClientName = "MainStorage";
    public const string ServiceBusClientName = "ServiceBus";
    public static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
}