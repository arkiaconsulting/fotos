namespace Fotos.App.Application.Folders;

internal sealed class RemoveFolderBusiness
{
    private readonly RemoveFolderFromStore _removeFolderFromStore;

    public RemoveFolderBusiness(RemoveFolderFromStore removeFolderFromStore)
    {
        _removeFolderFromStore = removeFolderFromStore;
    }

    public async Task Process(Guid parentId, Guid folderId)
    {
        await _removeFolderFromStore(parentId, folderId);
    }
}
