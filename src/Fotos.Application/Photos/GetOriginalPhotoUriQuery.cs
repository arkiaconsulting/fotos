namespace Fotos.Application.Photos;

public sealed record GetOriginalPhotoUriQuery(PhotoId PhotoId) : IQuery<Uri>
{
    internal sealed class Handler : IQueryHandler<GetOriginalPhotoUriQuery, Uri>
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
