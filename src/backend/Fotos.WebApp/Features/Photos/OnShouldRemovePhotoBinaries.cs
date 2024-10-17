using Fotos.WebApp.Types;
using Microsoft.Azure.WebJobs;

namespace Fotos.WebApp.Features.Photos;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Required by WebJobs runtime")]
public sealed class OnShouldRemovePhotoBinaries
{
    private readonly RemovePhotoOriginal _removePhotoOriginal;
    private readonly RemovePhotoThumbnail _removePhotoThumbnail;

    public OnShouldRemovePhotoBinaries(
        RemovePhotoOriginal removePhotoOriginal,
        RemovePhotoThumbnail removePhotoThumbnail)
    {
        _removePhotoOriginal = removePhotoOriginal;
        _removePhotoThumbnail = removePhotoThumbnail;
    }

    [FunctionName("OnShouldRemovePhotoBinaries")]
    public async Task Handle(
        [ServiceBusTrigger("%ServiceBus:MainTopic%", "%ServiceBus:PhotoRemovedSubscription%", AutoCompleteMessages = true, Connection = "ServiceBus")] PhotoId photoId)
    {
        await _removePhotoOriginal(photoId);
        await _removePhotoThumbnail(photoId);
    }
}
