using Fotos.App.Domain;

namespace Fotos.App.Application.Photos;

internal sealed class UpdatePhotoBusiness
{
    private readonly AddPhotoToStore _storePhotoData;
    private readonly GetPhotoFromStore _getPhoto;

    public UpdatePhotoBusiness(
        AddPhotoToStore storePhotoData,
        GetPhotoFromStore getPhoto)
    {
        _storePhotoData = storePhotoData;
        _getPhoto = getPhoto;
    }

    public async Task Process(PhotoId photoId, string title)
    {
        var photo = (await _getPhoto(photoId)).WithTitle(title);

        await _storePhotoData(photo);
    }
}
