using Fotos.App.Features.PhotoAlbums;

namespace Fotos.App.Api.PhotoAlbums;

internal sealed class GetFolderAlbumsBusiness
{
    private readonly GetFolderAlbumsFromStore _getFolderAlbumsFromStore;
    private readonly GetAlbumPhotoCountFromStore _getAlbumPhotoCountFromStore;

    public GetFolderAlbumsBusiness(
        GetFolderAlbumsFromStore getFolderAlbumsFromStore,
        GetAlbumPhotoCountFromStore getAlbumPhotoCountFromStore)
    {
        _getFolderAlbumsFromStore = getFolderAlbumsFromStore;
        _getAlbumPhotoCountFromStore = getAlbumPhotoCountFromStore;
    }

    public async Task<IReadOnlyCollection<AlbumAugmented>> Process(Guid folderId)
    {
        var albums = await _getFolderAlbumsFromStore(folderId);

        return await Task.WhenAll(albums.Select(GetPhotoCount).ToList());
    }

    private async Task<AlbumAugmented> GetPhotoCount(Album album)
    {
        return new AlbumAugmented(album, await _getAlbumPhotoCountFromStore(album.FolderId, album.Id));
    }
}

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

internal readonly record struct AlbumAugmented(Album Album, int PhotoCount);