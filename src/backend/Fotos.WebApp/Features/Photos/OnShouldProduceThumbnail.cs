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
        var photo = await _readOriginalPhoto(photoId);

        await using var thumbnailStream = await _createThumbnail(photo);

        await _addPhotoToThumbnailStorage(photoId, photo);

        await photo.Content.DisposeAsync();
    }
}
