namespace Fotos.Application.Albums;

public sealed record GetAlbumQuery(AlbumId AlbumId) : IQuery<AlbumAugmented>
{
    internal sealed class Handler : IQueryHandler<GetAlbumQuery, AlbumAugmented>
    {
        private readonly GetAlbumFromStore _getAlbumFromStore;
        private readonly GetAlbumPhotoCountFromStore _getAlbumPhotoCountFromStore;
        public Handler(
            GetAlbumFromStore getAlbumFromStore,
            GetAlbumPhotoCountFromStore getAlbumPhotoCountFromStore)
        {
            _getAlbumFromStore = getAlbumFromStore;
            _getAlbumPhotoCountFromStore = getAlbumPhotoCountFromStore;
        }

        async Task<Result<AlbumAugmented>> IQueryHandler<GetAlbumQuery, AlbumAugmented>.Handle(GetAlbumQuery query, CancellationToken cancellationToken)
        {
            var albumId = query.AlbumId;
            var album = await _getAlbumFromStore(albumId);

            var count = await _getAlbumPhotoCountFromStore(album.FolderId, album.Id);

            return new AlbumAugmented(album, await _getAlbumPhotoCountFromStore(album.FolderId, album.Id));
        }
    }
}
