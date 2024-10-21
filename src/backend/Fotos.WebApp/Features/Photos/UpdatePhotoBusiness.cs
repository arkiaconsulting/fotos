using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.Photos;

internal sealed class UpdatePhotoBusiness
{
    private readonly StorePhotoData _storePhotoData;
    private readonly GetPhoto _getPhoto;

    public UpdatePhotoBusiness(
        StorePhotoData storePhotoData,
        GetPhoto getPhoto)
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
