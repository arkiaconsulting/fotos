using Fotos.App.Domain;

namespace Fotos.App.Application.Folders;

internal sealed class CreateFolderBusiness
{
    private readonly AddFolderToStore _addFolderToStore;

    public CreateFolderBusiness(AddFolderToStore addFolderToStore)
    {
        _addFolderToStore = addFolderToStore;
    }

    public async Task Process(Guid parentFolderId, string folderName)
    {
        var folder = Folder.Create(Guid.NewGuid(), parentFolderId, folderName);

        await _addFolderToStore(folder);
    }
}
