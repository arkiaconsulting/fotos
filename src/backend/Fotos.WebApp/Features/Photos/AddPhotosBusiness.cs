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

    public async Task<Guid> Process(Guid folderId, Guid albumId, Stream photo, string contentType, string fileName)
    {
        var id = Guid.NewGuid();
        var photoId = new PhotoId(folderId, albumId, id);

        await _addPhotoToMainStorage(photoId, photo, contentType);

        var photoData = new PhotoEntity(photoId, fileName);

        await _storePhotoData(photoData);

        await _onNewPhotoUploaded(photoId);

        return id;
    }
}