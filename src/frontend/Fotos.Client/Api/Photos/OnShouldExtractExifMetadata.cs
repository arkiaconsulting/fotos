using Fotos.Client.Features.Photos;
using Fotos.Client.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.WebJobs;

namespace Fotos.Client.Api.Photos;

public sealed class OnShouldExtractExifMetadata
{
    private readonly ReadOriginalPhotoFromStorage _readOriginalPhoto;
    private readonly ExtractExifMetadata _extractExifMetadata;
    private readonly GetPhotoFromStore _getPhoto;
    private readonly AddPhotoToStore _storePhotoData;
    private readonly IHubContext<PhotosHub> _hubContext;

    public OnShouldExtractExifMetadata(
        ReadOriginalPhotoFromStorage readOriginalPhoto,
        ExtractExifMetadata extractExifMetadata,
        GetPhotoFromStore getPhoto,
        AddPhotoToStore storePhotoData,
        IHubContext<PhotosHub> hubContext)
    {
        _readOriginalPhoto = readOriginalPhoto;
        _extractExifMetadata = extractExifMetadata;
        _getPhoto = getPhoto;
        _storePhotoData = storePhotoData;
        _hubContext = hubContext;
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

            await _hubContext.Clients.All.SendAsync("MetadataReady", photoId);
        }
        catch (NotSupportedException)
        {
            // Should log warning here
        }
    }
}
