using Microsoft.Azure.WebJobs;

namespace Fotos.Client.Api.Photos;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Required by WebJobs runtime")]
public sealed class OnShouldProduceThumbnail
{
    private readonly ReadOriginalPhotoFromStorage _readOriginalPhoto;
    private readonly CreateThumbnail _createThumbnail;
    private readonly AddPhotoToThumbnailStorage _addPhotoToThumbnailStorage;
    private readonly OnThumbnailReady _onThumbnailReady;

    public OnShouldProduceThumbnail(
        ReadOriginalPhotoFromStorage readOriginalPhoto,
        CreateThumbnail createThumbnail,
        AddPhotoToThumbnailStorage addPhotoToThumbnailStorage,
        OnThumbnailReady onThumbnailReady)
    {
        _readOriginalPhoto = readOriginalPhoto;
        _createThumbnail = createThumbnail;
        _addPhotoToThumbnailStorage = addPhotoToThumbnailStorage;
        _onThumbnailReady = onThumbnailReady;
    }

    [FunctionName("OnShouldProduceThumbnail")]
    public async Task Handle(
        [ServiceBusTrigger("%ServiceBus:MainTopic%", "%ServiceBus:ProduceThumbnailSubscription%", AutoCompleteMessages = true, Connection = "ServiceBus")] PhotoId photoId)
    {
        var photo = await _readOriginalPhoto(photoId);

        await using var thumbnailStream = await _createThumbnail(photo);

        await _addPhotoToThumbnailStorage(photoId, new(thumbnailStream, photo.MimeType));

        await photo.Content.DisposeAsync();

        await _onThumbnailReady(photoId);
    }
}
