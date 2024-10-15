using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.Photos;

internal sealed class AddPhotosBusiness
{
    private readonly StorePhotoData _storePhotoData;
    private readonly AddPhotoToMainStorage _addPhotoToMainStorage;
    private readonly OnNewPhotoUploaded _onNewPhotoUploaded;

    public AddPhotosBusiness(
        StorePhotoData storePhotoData,
        AddPhotoToMainStorage addPhotoToMainStorage,
        OnNewPhotoUploaded onNewPhotoUploaded)
    {
        _storePhotoData = storePhotoData;
        _addPhotoToMainStorage = addPhotoToMainStorage;
        _onNewPhotoUploaded = onNewPhotoUploaded;
    }

    public async Task Process(Guid folderId, Guid albumId, Stream photo)
    {
        var photoId = new PhotoId(folderId, albumId, Guid.NewGuid());

        await _addPhotoToMainStorage(photoId, photo);

        var photoData = new PhotoEntity(photoId);

        await _storePhotoData(photoData);

        await _onNewPhotoUploaded(photoId);
    }
}