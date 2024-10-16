using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.Photos;

internal sealed class OnShouldProduceThumbnail
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

    public async Task Handle(PhotoId photoId)
    {
        var (originalStream, mimeType) = await _readOriginalPhoto(photoId);

        await using var thumbnailStream = await _createThumbnail(originalStream, mimeType);

        await _addPhotoToThumbnailStorage(photoId, thumbnailStream);
    }
}
