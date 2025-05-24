using Fotos.App.Domain;

namespace Fotos.App.Application.Photos;

internal sealed record GetPhotoThumbnailUriQuery(PhotoId PhotoId) : IQuery<Uri>
{
    public sealed class Handler : IQueryHandler<GetPhotoThumbnailUriQuery, Uri>
    {
        private readonly GetThumbnailStorageUri _getThumbnailStorageUri;

        public Handler(GetThumbnailStorageUri getThumbnailStorageUri)
        {
            _getThumbnailStorageUri = getThumbnailStorageUri;
        }

        async Task<Result<Uri>> IQueryHandler<GetPhotoThumbnailUriQuery, Uri>.Handle(GetPhotoThumbnailUriQuery query, CancellationToken cancellationToken)
        {
            return await _getThumbnailStorageUri(query.PhotoId);
        }
    }
}
