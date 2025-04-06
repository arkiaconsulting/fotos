using Fotos.App.Api.PhotoFolders;
using Fotos.App.Api.Types;

namespace Fotos.App.Application.Folders;

internal sealed class GetFolderBusiness
{
    private readonly GetFolderFromStore _getFolderFromStore;

    public GetFolderBusiness(GetFolderFromStore getFolderFromStore)
    {
        _getFolderFromStore = getFolderFromStore;
    }

    public async Task<Folder> Process(Guid parentId, Guid folderId)
    {
        return await _getFolderFromStore(parentId, folderId);
    }
}
