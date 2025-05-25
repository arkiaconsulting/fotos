using Fotos.Application;
using Fotos.Application.Photos;
using Fotos.Core;
using Microsoft.Azure.WebJobs;
using System.Diagnostics;

namespace Fotos.App.Functions;

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
        [ServiceBusTrigger("%ServiceBus:MainTopic%", "%ServiceBus:RemovePhotoBinariesSubscription%", Connection = "ServiceBus")] PhotoId photoId)
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity(
            ActivityKind.Consumer,
            tags: [new("photo.id", photoId.ToString())],
            name: "OnShouldRemovePhotoBinaries");

        await _removePhotoOriginal(photoId);
        await _removePhotoThumbnail(photoId);

        activity?.AddEvent(new("Photo binaries removed successfully"));
    }
}
