using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.Photos;

internal sealed class OnShouldExtractExifMetadata
{
    private readonly ReadOriginalPhoto _readOriginalPhoto;
    private readonly ExtractExifMetadata _extractExifMetadata;
    private readonly GetPhoto _getPhoto;
    private readonly StorePhotoData _storePhotoData;

    public OnShouldExtractExifMetadata(
        ReadOriginalPhoto readOriginalPhoto,
        ExtractExifMetadata extractExifMetadata,
        GetPhoto getPhoto,
        StorePhotoData storePhotoData)
    {
        _readOriginalPhoto = readOriginalPhoto;
        _extractExifMetadata = extractExifMetadata;
        _getPhoto = getPhoto;
        _storePhotoData = storePhotoData;
    }

    public async Task Handle(PhotoId photoId)
    {
        var (stream, _) = await _readOriginalPhoto(photoId);

        var metadata = await _extractExifMetadata(stream);

        var photo = await _getPhoto(photoId);

        await _storePhotoData(photo = photo.WithMetadata(metadata));
    }
}
