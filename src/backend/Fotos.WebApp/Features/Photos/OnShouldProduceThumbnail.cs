using Fotos.WebApp.Types;
using Microsoft.Azure.WebJobs;

namespace Fotos.WebApp.Features.Photos;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Required by WebJobs runtime")]
public sealed class OnShouldProduceThumbnail
{
    private readonly ReadOriginalPhoto _readOriginalPhoto;
    private readonly CreateThumbnail _createThumbnail;
    private readonly AddPhotoToThumbnailStorage _addPhotoToThumbnailStorage;

    public OnShouldProduceThumbnail(
        ReadOriginalPhoto readOriginalPhoto,
        CreateThumbnail createThumbnail,
        AddPhotoToThumbnailStorage addPhotoToThumbnailStorage)
    {
        _readOriginalPhoto = readOriginalPhoto;
        _createThumbnail = createThumbnail;
        _addPhotoToThumbnailStorage = addPhotoToThumbnailStorage;
    }

    [FunctionName("OnShouldProduceThumbnail")]
    public async Task Handle(
        [ServiceBusTrigger("%ServiceBus:MainTopic%", "%ServiceBus:PhotoUploadedSubscription%", AutoCompleteMessages = true, Connection = "ServiceBus")] PhotoId photoId)
    {
        var photo = await _readOriginalPhoto(photoId);

        await using var thumbnailStream = await _createThumbnail(photo);

        await _addPhotoToThumbnailStorage(photoId, new(thumbnailStream, photo.MimeType));

        await photo.Content.DisposeAsync();
    }
}
