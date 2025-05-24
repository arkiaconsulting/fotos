using Fotos.App.Domain;

namespace Fotos.App.Application.Photos;

internal sealed record ListAlbumPhotosQuery(AlbumId AlbumId) : IQuery<IReadOnlyCollection<Photo>>
{
    public sealed class Handler : IQueryHandler<ListAlbumPhotosQuery, IReadOnlyCollection<Photo>>
    {
        private readonly ListPhotosFromStore _listPhotosFromStore;

        public Handler(ListPhotosFromStore listPhotosFromStore)
        {
            _listPhotosFromStore = listPhotosFromStore;
        }

        async Task<Result<IReadOnlyCollection<Photo>>> IQueryHandler<ListAlbumPhotosQuery, IReadOnlyCollection<Photo>>.Handle(ListAlbumPhotosQuery query, CancellationToken cancellationToken)
        {
            return Result.Of(await _listPhotosFromStore(query.AlbumId));
        }
    }
}
