namespace Fotos.Application.Folders;

public sealed record GetFolderQuery(Guid ParentFolderId, Guid FolderId)
    : IQuery<Folder>
{
    internal sealed class Handler : IQueryHandler<GetFolderQuery, Folder>
    {
        private readonly GetFolderFromStore _getFolderFromStore;

        public Handler(GetFolderFromStore getFolderFromStore)
        {
            _getFolderFromStore = getFolderFromStore;
        }

        public async Task<Result<Folder>> Handle(GetFolderQuery query, CancellationToken cancellationToken) =>
            await _getFolderFromStore(query.ParentFolderId, query.FolderId);
    }
}
