using Fotos.App.Api.PhotoFolders;
using Fotos.App.Api.Types;

namespace Fotos.App.Application.Folders;

internal sealed class ListChildFoldersBusiness
{
    private readonly GetFoldersFromStore _getFoldersFromStore;

    public ListChildFoldersBusiness(GetFoldersFromStore getFoldersFromStore)
    {
        _getFoldersFromStore = getFoldersFromStore;
    }

    public async Task<IEnumerable<Folder>> Process(Guid parentFolderId)
    {
        return await _getFoldersFromStore(parentFolderId);
    }
}
