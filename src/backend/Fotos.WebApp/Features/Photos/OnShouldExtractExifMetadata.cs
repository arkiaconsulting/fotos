using Microsoft.Azure.WebJobs;

namespace Fotos.WebApp.Features.Photos;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Required by WebJobs runtime")]
public sealed class OnShouldExtractExifMetadata
{
    private readonly ReadOriginalPhotoFromStorage _readOriginalPhoto;
    private readonly ExtractExifMetadata _extractExifMetadata;
    private readonly GetPhotoFromStore _getPhoto;
    private readonly AddPhotoToStore _storePhotoData;

    public OnShouldExtractExifMetadata(
        ReadOriginalPhotoFromStorage readOriginalPhoto,
        ExtractExifMetadata extractExifMetadata,
        GetPhotoFromStore getPhoto,
        AddPhotoToStore storePhotoData)
    {
        _readOriginalPhoto = readOriginalPhoto;
        _extractExifMetadata = extractExifMetadata;
        _getPhoto = getPhoto;
        _storePhotoData = storePhotoData;
    }

    [FunctionName("OnShouldExtractExifMetadata")]
    public async Task Handle(
        [ServiceBusTrigger("%ServiceBus:MainTopic%", "%ServiceBus:ExtractExifMetadataSubscription%", AutoCompleteMessages = true, Connection = "ServiceBus")] PhotoId photoId)
    {
        var (stream, mimeType) = await _readOriginalPhoto(photoId);

        try
        {
            var metadata = await _extractExifMetadata(stream, mimeType);

            var photo = await _getPhoto(photoId);

            await _storePhotoData(photo = photo.WithMetadata(metadata));

            await stream.DisposeAsync();
        }
        catch (NotSupportedException)
        {
            // Should log warning here
        }
    }
}
