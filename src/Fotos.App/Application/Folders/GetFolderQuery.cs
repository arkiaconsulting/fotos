using Fotos.App.Domain;

namespace Fotos.App.Application.Folders;

internal sealed record GetFolderQuery(Guid ParentFolderId, Guid FolderId)
    : IQuery<Folder>
{
    public sealed class Handler : IQueryHandler<GetFolderQuery, Folder>
    {
        private readonly GetFolderFromStore _getFolderFromStore;

        public Handler(GetFolderFromStore getFolderFromStore)
        {
            _getFolderFromStore = getFolderFromStore;
        }

        public async Task<Result<Folder>> Handle(GetFolderQuery query, CancellationToken cancellationToken)
        {
            return await _getFolderFromStore(query.ParentFolderId, query.FolderId);
        }
    }
}
