using Fotos.App.Api.Account;
using Fotos.App.Api.Types;

namespace Fotos.App.Application.User;

internal sealed class FindUserBusiness
{
    private readonly FindUserInStore _findUserInStore;

    public FindUserBusiness(FindUserInStore findUserInStore)
    {
        _findUserInStore = findUserInStore;
    }

    public async Task<FotoUser?> Process(string provider, string userProviderId)
    {
        var userId = FotoUserId.Create(provider, userProviderId);

        return await _findUserInStore(userId);
    }
}
