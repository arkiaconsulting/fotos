using Fotos.App.Domain;

namespace Fotos.App.Application.Albums;

internal sealed record ListFolderAlbumsQuery(Guid FolderId) : IQuery<IReadOnlyCollection<AlbumAugmented>>
{
    public sealed class Handler : IQueryHandler<ListFolderAlbumsQuery, IReadOnlyCollection<AlbumAugmented>>
    {
        private readonly GetFolderAlbumsFromStore _getFolderAlbumsFromStore;
        private readonly GetAlbumPhotoCountFromStore _getAlbumPhotoCountFromStore;

        public Handler(
            GetFolderAlbumsFromStore getFolderAlbumsFromStore,
            GetAlbumPhotoCountFromStore getAlbumPhotoCountFromStore)
        {
            _getFolderAlbumsFromStore = getFolderAlbumsFromStore;
            _getAlbumPhotoCountFromStore = getAlbumPhotoCountFromStore;
        }

        public async Task<Result<IReadOnlyCollection<AlbumAugmented>>> Handle(ListFolderAlbumsQuery query, CancellationToken cancellationToken)
        {
            var albums = await _getFolderAlbumsFromStore(query.FolderId);

            return await Task.WhenAll(albums.Select(GetPhotoCount).ToList());
        }

        private async Task<AlbumAugmented> GetPhotoCount(Album album)
        {
            return new AlbumAugmented(album, await _getAlbumPhotoCountFromStore(album.FolderId, album.Id));
        }
    }

}
