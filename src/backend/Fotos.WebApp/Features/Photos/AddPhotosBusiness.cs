namespace Fotos.WebApp.Features.Photos;

internal sealed class AddPhotosBusiness
{
    private readonly AddPhotoToStore _storePhotoData;
    private readonly AddPhotoToMainStorage _addPhotoToMainStorage;
    private readonly OnNewPhotoUploaded _onNewPhotoUploaded;

    public AddPhotosBusiness(
        AddPhotoToStore storePhotoData,
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

        var photoData = new Photo(photoId, fileName);

        await _storePhotoData(photoData);

        await _onNewPhotoUploaded(photoId);

        return id;
    }
}