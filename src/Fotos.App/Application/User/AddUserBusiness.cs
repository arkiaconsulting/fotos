using Fotos.App.Application.Folders;
using Fotos.App.Domain;

namespace Fotos.App.Application.User;

internal sealed class AddUserBusiness
{
    private readonly FindUserInStore _findUserInStore;
    private readonly AddUserToStore _addUserToStore;
    private readonly AddFolderToStore _addFolderToStore;

    public AddUserBusiness(
        FindUserInStore findUserInStore,
        AddUserToStore addUserToStore,
        AddFolderToStore addFolderToStore)
    {
        _findUserInStore = findUserInStore;
        _addUserToStore = addUserToStore;
        _addFolderToStore = addFolderToStore;
    }

    public async Task Process(FotoUserId userId, string givenName)
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity("BUSINESS: add user", System.Diagnostics.ActivityKind.Internal);

        var existingUser = await _findUserInStore(userId);

        if (existingUser is not null)
        {
            activity?.AddEvent(new("update existing user"));

            await _addUserToStore(existingUser.Value with { GivenName = Name.Create(givenName) });

            return;
        }

        activity?.AddEvent(new("create new user"));

        var rootFolderId = Guid.NewGuid();
        var parentFolderId = Guid.Empty;
        await _addUserToStore(new FotoUser(userId, Name.Create(givenName), rootFolderId));

        await _addFolderToStore(new Folder(rootFolderId, parentFolderId, Name.Create("Root")));
    }
}
