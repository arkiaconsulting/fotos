using Fotos.App.Domain;

namespace Fotos.App.Application.Albums;

internal sealed class GetAlbumBusiness
{
    private readonly GetAlbumFromStore _getAlbumFromStore;
    private readonly GetAlbumPhotoCountFromStore _getAlbumPhotoCountFromStore;

    public GetAlbumBusiness(GetAlbumFromStore getAlbumFromStore,
        GetAlbumPhotoCountFromStore getAlbumPhotoCountFromStore)
    {
        _getAlbumFromStore = getAlbumFromStore;
        _getAlbumPhotoCountFromStore = getAlbumPhotoCountFromStore;
    }

    public async Task<AlbumAugmented> Process(AlbumId albumId)
    {
        var album = await _getAlbumFromStore(albumId);

        return new AlbumAugmented(album, await _getAlbumPhotoCountFromStore(album.FolderId, album.Id));
    }
}
