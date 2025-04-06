using Fotos.App.Domain;

namespace Fotos.App.Application.Photos;

internal sealed class GetOriginalPhotoUriBusiness
{
    private readonly GetOriginalStorageUri _getOriginalStorageUri;

    public GetOriginalPhotoUriBusiness(GetOriginalStorageUri getOriginalStorageUri)
    {
        _getOriginalStorageUri = getOriginalStorageUri;
    }

    public async Task<Uri> Process(PhotoId photoId)
    {
        return await _getOriginalStorageUri(photoId);
    }
}
