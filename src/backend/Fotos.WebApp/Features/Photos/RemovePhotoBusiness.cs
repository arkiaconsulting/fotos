using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.Photos;

internal sealed class RemovePhotoBusiness
{
    private readonly RemovePhotoData _removePhotoData;
    private readonly OnPhotoRemoved _onPhotoRemoved;

    public RemovePhotoBusiness(
        RemovePhotoData removePhotoData,
        OnPhotoRemoved onPhotoRemoved)
    {
        _removePhotoData = removePhotoData;
        _onPhotoRemoved = onPhotoRemoved;
    }

    public async Task Process(PhotoId photoId)
    {
        await _removePhotoData(photoId);

        await _onPhotoRemoved(photoId);
    }
}
