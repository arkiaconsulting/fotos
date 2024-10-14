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
        var photoId = Guid.NewGuid();

        await _addPhotoToMainStorage(photoId, photo);

        var photoData = new PhotoEntity(folderId, albumId, photoId, new Uri("https://example.com/photo.jpg"));

        await _storePhotoData(photoData);

        await _onNewPhotoUploaded(photoId);
    }
}