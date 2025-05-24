using Fotos.App.Domain;

namespace Fotos.App.Application.Photos;

internal sealed record GetOriginalPhotoUriQuery(PhotoId PhotoId) : IQuery<Uri>
{
    public sealed class Handler : IQueryHandler<GetOriginalPhotoUriQuery, Uri>
    {
        private readonly GetOriginalStorageUri _getOriginalStorageUri;

        public Handler(GetOriginalStorageUri getOriginalStorageUri)
        {
            _getOriginalStorageUri = getOriginalStorageUri;
        }

        async Task<Result<Uri>> IQueryHandler<GetOriginalPhotoUriQuery, Uri>.Handle(GetOriginalPhotoUriQuery query, CancellationToken cancellationToken)
        {
            return await _getOriginalStorageUri(query.PhotoId);
        }
    }
}
