using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.Photos;

internal sealed class RemovePhotoBusiness
{
    private readonly RemovePhotoData _removePhotoData;

    public RemovePhotoBusiness(RemovePhotoData removePhotoData)
    {
        _removePhotoData = removePhotoData;
    }

    public async Task Process(PhotoId photoId)
    {
        await _removePhotoData(photoId);
    }
}
