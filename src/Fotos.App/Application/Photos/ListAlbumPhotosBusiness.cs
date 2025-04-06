using Fotos.App.Domain;

namespace Fotos.App.Application.Photos;

internal sealed class ListAlbumPhotosBusiness
{
    private readonly ListPhotosFromStore _listPhotosFromStore;

    public ListAlbumPhotosBusiness(ListPhotosFromStore listPhotosFromStore)
    {
        _listPhotosFromStore = listPhotosFromStore;
    }

    public async Task<IEnumerable<Photo>> Process(AlbumId albumId)
    {
        return await _listPhotosFromStore(albumId);
    }
}
