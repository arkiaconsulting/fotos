using Fotos.App.Domain;

namespace Fotos.App.Application.Photos;

internal sealed record GetPhotoQuery(PhotoId PhotoId) : IQuery<Photo>
{
    public sealed class Handler : IQueryHandler<GetPhotoQuery, Photo>
    {
        private readonly GetPhotoFromStore _getPhotoFromStore;

        public Handler(GetPhotoFromStore getPhotoFromStore)
        {
            _getPhotoFromStore = getPhotoFromStore;
        }

        async Task<Result<Photo>> IQueryHandler<GetPhotoQuery, Photo>.Handle(GetPhotoQuery query, CancellationToken cancellationToken)
        {
            return await _getPhotoFromStore(query.PhotoId);
        }
    }
}
