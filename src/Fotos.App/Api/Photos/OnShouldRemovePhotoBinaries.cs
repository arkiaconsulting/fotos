using Fotos.App.Features.Photos;
using Microsoft.Azure.WebJobs;

namespace Fotos.App.Api.Photos;

public sealed class OnShouldRemovePhotoBinaries
{
    private readonly RemovePhotoOriginalFromStorage _removePhotoOriginal;
    private readonly RemovePhotoThumbnailFromStorage _removePhotoThumbnail;

    public OnShouldRemovePhotoBinaries(
        RemovePhotoOriginalFromStorage removePhotoOriginal,
        RemovePhotoThumbnailFromStorage removePhotoThumbnail)
    {
        _removePhotoOriginal = removePhotoOriginal;
        _removePhotoThumbnail = removePhotoThumbnail;
    }

    [FunctionName("OnShouldRemovePhotoBinaries")]
    public async Task Handle(
        [ServiceBusTrigger("%ServiceBus:MainTopic%", "%ServiceBus:RemovePhotoBinariesSubscription%", AutoCompleteMessages = true, Connection = "ServiceBus")] PhotoId photoId)
    {
        await _removePhotoOriginal(photoId);
        await _removePhotoThumbnail(photoId);
    }
}
