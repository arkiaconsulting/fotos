using Fotos.Client.Api.Shared;
using Fotos.Client.Api.Types;

namespace Fotos.Client.Api.Account;

internal sealed class AddUserBusiness
{
    private readonly AddUserToStore _addUserToStore;

    public AddUserBusiness(AddUserToStore addUserToStore) => _addUserToStore = addUserToStore;

    public async Task Process(string provider, string providerId, string givenName)
    {
        var fotoUser = new FotoUser(FotoUserId.Create(provider, providerId), Name.Create(givenName));

        await _addUserToStore(fotoUser);
    }
}
