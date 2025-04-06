using Fotos.App.Api.Account;
using Fotos.App.Api.PhotoFolders;
using Fotos.App.Api.Shared;
using Fotos.App.Api.Types;

namespace Fotos.App.Application.User;

internal sealed class AddUserBusiness
{
    private readonly FindUserInStore _findUserInStore;
    private readonly AddUserToStore _addUserToStore;
    private readonly AddFolderToStore _addFolderToStore;
    private readonly InstrumentationConfig _instrumentation;

    public AddUserBusiness(
        FindUserInStore findUserInStore,
        AddUserToStore addUserToStore,
        AddFolderToStore addFolderToStore,
        InstrumentationConfig instrumentation)
    {
        _findUserInStore = findUserInStore;
        _addUserToStore = addUserToStore;
        _addFolderToStore = addFolderToStore;
        _instrumentation = instrumentation;
    }

    public async Task Process(string provider, string providerId, string givenName)
    {
        using var activity = _instrumentation.ActivitySource.StartActivity("BUSINESS: add user", System.Diagnostics.ActivityKind.Internal);

        var userId = FotoUserId.Create(provider, providerId);
        var existingUser = await _findUserInStore(userId);

        if (existingUser is not null)
        {
            activity?.AddEvent(new("update existing user"));

            await _addUserToStore(existingUser.Value with { GivenName = Name.Create(givenName) });

            return;
        }

        activity?.AddEvent(new("create new user"));

        var rootFolderId = Guid.NewGuid();
        await _addUserToStore(new FotoUser(userId, Name.Create(givenName), rootFolderId));

        await _addFolderToStore(new Folder(Guid.NewGuid(), rootFolderId, Name.Create("Root")));
    }
}
