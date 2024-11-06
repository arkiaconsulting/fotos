using Fotos.Client.Api.Types;
using System.Diagnostics.CodeAnalysis;

namespace Fotos.WebApp.Tests.Assets.InMemory.DataStore;

[SuppressMessage("Design", "CA1812", Justification = "Instantiated by DI")]
internal sealed class InMemoryUserDataStore
{
    private readonly List<FotoUser> _source;

    public InMemoryUserDataStore(List<FotoUser> source) => _source = source;

    public Task Add(FotoUser entity)
    {
        _source.Add(entity);

        return Task.CompletedTask;
    }

    public Task<FotoUser?> Find(FotoUserId id)
    {
        var filtered = _source.Where(x => x.Id == id).ToList();

        if (filtered.Count != 0)
        {
            return Task.FromResult<FotoUser?>(filtered[0]);
        }

        return Task.FromResult<FotoUser?>(default);
    }
}
