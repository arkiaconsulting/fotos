using Fotos.App.Adapters.RealTimeMessaging;
using Fotos.Application;
using Fotos.Application.Photos;
using Fotos.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.WebJobs;
using System.Diagnostics;

namespace Fotos.App.Functions;

public sealed class OnShouldExtractExifMetadata
{
    private readonly ReadOriginalPhotoFromStorage _readOriginalPhoto;
    private readonly ExtractExifMetadata _extractExifMetadata;
    private readonly GetPhotoFromStore _getPhoto;
    private readonly AddPhotoToStore _storePhotoData;
    private readonly IHubContext<PhotosHub> _hubContext;
    private readonly ILogger<OnShouldExtractExifMetadata> _logger;

    public OnShouldExtractExifMetadata(
        ReadOriginalPhotoFromStorage readOriginalPhoto,
        ExtractExifMetadata extractExifMetadata,
        GetPhotoFromStore getPhoto,
        AddPhotoToStore storePhotoData,
        IHubContext<PhotosHub> hubContext,
        ILogger<OnShouldExtractExifMetadata> logger)
    {
        _readOriginalPhoto = readOriginalPhoto;
        _extractExifMetadata = extractExifMetadata;
        _getPhoto = getPhoto;
        _storePhotoData = storePhotoData;
        _hubContext = hubContext;
        _logger = logger;
    }

    [FunctionName("OnShouldExtractExifMetadata")]
    public async Task Handle(
        [ServiceBusTrigger("%ServiceBus:MainTopic%", "%ServiceBus:ExtractExifMetadataSubscription%", Connection = "ServiceBus")] PhotoId photoId)
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity(
            ActivityKind.Consumer,
            tags: [new("photo.id", photoId.ToString())],
            name: "OnShouldExtractExifMetadata");

        var (stream, mimeType) = await _readOriginalPhoto(photoId);

        var metadata = await _extractExifMetadata(stream, mimeType);

        var photo = await _getPhoto(photoId);

        await _storePhotoData(photo = photo.WithMetadata(metadata));

        await stream.DisposeAsync();

        await _hubContext.Clients.All.SendAsync("MetadataReady", photoId);

        activity?.AddEvent(new("EXIF metadata extracted successfully"));
    }
}
