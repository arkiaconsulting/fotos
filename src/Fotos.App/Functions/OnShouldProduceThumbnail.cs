using Fotos.App.Adapters.RealTimeMessaging;
using Fotos.Application;
using Fotos.Application.Photos;
using Fotos.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.WebJobs;
using System.Diagnostics;

namespace Fotos.App.Functions;

public sealed class OnShouldProduceThumbnail
{
    private readonly ReadOriginalPhotoFromStorage _readOriginalPhoto;
    private readonly CreateThumbnail _createThumbnail;
    private readonly AddPhotoToThumbnailStorage _addPhotoToThumbnailStorage;
    private readonly IHubContext<PhotosHub> _hubContext;

    public OnShouldProduceThumbnail(
        ReadOriginalPhotoFromStorage readOriginalPhoto,
        CreateThumbnail createThumbnail,
        AddPhotoToThumbnailStorage addPhotoToThumbnailStorage,
        IHubContext<PhotosHub> hubContext)
    {
        _readOriginalPhoto = readOriginalPhoto;
        _createThumbnail = createThumbnail;
        _addPhotoToThumbnailStorage = addPhotoToThumbnailStorage;
        _hubContext = hubContext;
    }

    [FunctionName("OnShouldProduceThumbnail")]
    public async Task Handle(
        [ServiceBusTrigger("%ServiceBus:MainTopic%", "%ServiceBus:ProduceThumbnailSubscription%", Connection = "ServiceBus")] PhotoId photoId)
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity(
            ActivityKind.Consumer,
            tags: [new("photo.id", photoId.ToString())],
            name: "OnShouldProduceThumbnail");

        var photo = await _readOriginalPhoto(photoId);

        await using var thumbnailStream = await _createThumbnail(photo);

        await _addPhotoToThumbnailStorage(photoId, new(thumbnailStream, photo.MimeType));

        await photo.Content.DisposeAsync();

        await _hubContext.Clients.All.SendAsync("ThumbnailReady", photoId);

        activity?.AddEvent(new("Thumbnail produced with success"));
    }
}
