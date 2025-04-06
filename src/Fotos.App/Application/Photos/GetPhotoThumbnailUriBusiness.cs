using Fotos.App.Domain;

namespace Fotos.App.Application.Photos;

internal sealed class GetPhotoThumbnailUriBusiness
{
    private readonly GetThumbnailStorageUri _getThumbnailStorageUri;

    public GetPhotoThumbnailUriBusiness(GetThumbnailStorageUri getThumbnailStorageUri)
    {
        _getThumbnailStorageUri = getThumbnailStorageUri;
    }

    public async Task<Uri> Process(PhotoId photoId)
    {
        return await _getThumbnailStorageUri(photoId);
    }
}