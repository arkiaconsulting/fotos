using Fotos.App.Domain;

namespace Fotos.App.Application.Folders;

internal sealed record ListChildFoldersQuery(Guid FolderId) : IQuery<IReadOnlyCollection<Folder>>
{
    public sealed class Handler : IQueryHandler<ListChildFoldersQuery, IReadOnlyCollection<Folder>>
    {
        private readonly GetFoldersFromStore _getFoldersFromStore;

        public Handler(GetFoldersFromStore getFoldersFromStore)
        {
            _getFoldersFromStore = getFoldersFromStore;
        }

        public async Task<Result<IReadOnlyCollection<Folder>>> Handle(ListChildFoldersQuery query, CancellationToken cancellationToken)
        {
            var folders = await _getFoldersFromStore(query.FolderId);

            return Result.Of(folders);
        }
    }
}
