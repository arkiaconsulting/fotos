using Fotos.App.Api.PhotoAlbums;

namespace Fotos.App.Application.Albums;

internal sealed class ListFolderAlbumsBusiness
{
    private readonly GetFolderAlbumsFromStore _getFolderAlbumsFromStore;
    private readonly GetAlbumPhotoCountFromStore _getAlbumPhotoCountFromStore;

    public ListFolderAlbumsBusiness(
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

internal readonly record struct AlbumAugmented(Album Album, int PhotoCount);