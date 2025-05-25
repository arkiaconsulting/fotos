using Fotos.Core;
using System.Diagnostics.CodeAnalysis;

namespace Fotos.Tests.Assets.InMemory.DataStore;

[SuppressMessage("Design", "CA1812", Justification = "Instantiated by DI")]
internal sealed class InMemoryFolderDataStore
{
    private readonly List<Folder> _source;

    public InMemoryFolderDataStore(List<Folder> source) => _source = source;

    public Task Add(Folder entity)
    {
        _source.Add(entity);

        return Task.CompletedTask;
    }

    public Task Remove(Guid _, Guid id)
    {
        _source.RemoveAll(folder => folder.Id == id);

        return Task.CompletedTask;
    }

    public Task<Folder> Get(Guid _, Guid id) =>
        Task.FromResult(_source.Single(folder => folder.Id == id));

    public Task<IReadOnlyCollection<Folder>> GetByParent(Guid parentId) =>
        Task.FromResult<IReadOnlyCollection<Folder>>([.. _source.Where(folder => folder.ParentId == parentId)]);

    public Task Update(Guid parentId, Guid id, Name name)
    {
        var existing = _source.First(x => x.Id == id);
        _source.Remove(existing);

        _source.Add(new(id, parentId, name));

        return Task.CompletedTask;
    }
}
