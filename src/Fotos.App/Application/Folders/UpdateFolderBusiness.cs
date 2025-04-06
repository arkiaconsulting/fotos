using Fotos.App.Api.PhotoFolders;
using Fotos.App.Api.Shared;

namespace Fotos.App.Application.Folders;

internal sealed class UpdateFolderBusiness
{
    private readonly UpdateFolderInStore _updateFolderInStore;

    public UpdateFolderBusiness(UpdateFolderInStore updateFolderInStore)
    {
        _updateFolderInStore = updateFolderInStore;
    }

    public async Task Process(Guid parentId, Guid folderId, string newName)
    {
        await _updateFolderInStore(parentId, folderId, Name.Create(newName));
    }
}
