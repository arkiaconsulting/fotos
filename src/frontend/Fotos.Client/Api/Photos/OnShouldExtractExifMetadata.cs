using Fotos.Client.Features.Photos;
using Microsoft.Azure.WebJobs;

namespace Fotos.Client.Api.Photos;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Required by WebJobs runtime")]
public sealed class OnShouldExtractExifMetadata
{
    private readonly ReadOriginalPhotoFromStorage _readOriginalPhoto;
    private readonly ExtractExifMetadata _extractExifMetadata;
    private readonly GetPhotoFromStore _getPhoto;
    private readonly AddPhotoToStore _storePhotoData;
    private readonly OnMetadataReady _onMetadataReady;

    public OnShouldExtractExifMetadata(
        ReadOriginalPhotoFromStorage readOriginalPhoto,
        ExtractExifMetadata extractExifMetadata,
        GetPhotoFromStore getPhoto,
        AddPhotoToStore storePhotoData,
        OnMetadataReady onMetadataReady)
    {
        _readOriginalPhoto = readOriginalPhoto;
        _extractExifMetadata = extractExifMetadata;
        _getPhoto = getPhoto;
        _storePhotoData = storePhotoData;
        _onMetadataReady = onMetadataReady;
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

            await _onMetadataReady(photoId);
        }
        catch (NotSupportedException)
        {
            // Should log warning here
        }
    }
}
