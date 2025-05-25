namespace Fotos.Application.Folders;

public sealed record ListChildFoldersQuery(Guid FolderId) : IQuery<IReadOnlyCollection<Folder>>
{
    internal sealed class Handler : IQueryHandler<ListChildFoldersQuery, IReadOnlyCollection<Folder>>
    {
        private readonly GetFoldersFromStore _getFoldersFromStore;

        public Handler(GetFoldersFromStore getFoldersFromStore)
        {
            _getFoldersFromStore = getFoldersFromStore;
        }

        async Task<Result<IReadOnlyCollection<Folder>>> IQueryHandler<ListChildFoldersQuery, IReadOnlyCollection<Folder>>.Handle(ListChildFoldersQuery query, CancellationToken cancellationToken)
        {
            var folders = await _getFoldersFromStore(query.FolderId);

            return Result.Of(folders);
        }
    }
}
