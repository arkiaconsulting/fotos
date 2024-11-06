using Fotos.Client.Features.Photos;
using Fotos.Client.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.WebJobs;

namespace Fotos.Client.Api.Photos;

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
        [ServiceBusTrigger("%ServiceBus:MainTopic%", "%ServiceBus:ProduceThumbnailSubscription%", AutoCompleteMessages = true, Connection = "ServiceBus")] PhotoId photoId)
    {
        var photo = await _readOriginalPhoto(photoId);

        await using var thumbnailStream = await _createThumbnail(photo);

        await _addPhotoToThumbnailStorage(photoId, new(thumbnailStream, photo.MimeType));

        await photo.Content.DisposeAsync();

        await _hubContext.Clients.All.SendAsync("ThumbnailReady", photoId);
    }
}
