using Fotos.App.Domain;

namespace Fotos.App.Application.Photos;

internal sealed class GetPhotoBusiness
{
    private readonly GetPhotoFromStore _getPhoto;

    public GetPhotoBusiness(GetPhotoFromStore getPhoto) => _getPhoto = getPhoto;

    public async Task<Photo> Process(PhotoId photoId)
    {
        return await _getPhoto(photoId);
    }
}
