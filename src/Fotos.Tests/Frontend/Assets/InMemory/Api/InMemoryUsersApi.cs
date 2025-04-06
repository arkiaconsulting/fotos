using Fotos.App.Api.Shared;
using Fotos.App.Api.Types;
using Fotos.Tests.Backend.Assets.InMemory.DataStore;
using System.Diagnostics.CodeAnalysis;

namespace Fotos.Tests.Frontend.Assets.InMemory.Api;

[SuppressMessage("Design", "CA1812", Justification = "Instantiated by DI")]
internal sealed class InMemoryUsersApi
{
    private readonly List<FotoUser> _entities;
    private readonly Guid _rootFolderId;

    public InMemoryUsersApi(List<FotoUser> entities, RootFolderIdWrapper rootFolderId)
    {
        _entities = entities;
        _rootFolderId = rootFolderId.Value;
    }

    public Task Add(string givenName)
    {
        var user = new FotoUser(FotoUserId.Create("provider", "id"), Name.Create(givenName), _rootFolderId);
        _entities.Add(user);

        return Task.FromResult(user);
    }
}
