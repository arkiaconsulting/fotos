using Fotos.App.Api.PhotoFolders;
using Fotos.App.Api.Shared;
using System.Diagnostics.CodeAnalysis;

namespace Fotos.Tests.Frontend.Assets.InMemory.Api;

[SuppressMessage("Design", "CA1812", Justification = "Instantiated by DI")]
internal sealed class InMemoryFoldersApi
{
    private readonly List<Folder> _entities;

    public InMemoryFoldersApi(List<Folder> entities)
    {
        _entities = entities;
    }

    public Task Add(Guid parentId, string name)
    {
        var folder = new Folder(Guid.NewGuid(), parentId, Name.Create(name));
        _entities.Add(folder);

        return Task.FromResult(folder);
    }

    public Task<IReadOnlyCollection<FolderDto>> List(Guid parentId)
    {
        var result = _entities
            .Where(f => f.ParentId == parentId)
            .Select(f => new FolderDto(f.Id, f.ParentId, f.Name.Value))
            .ToList();

        return Task.FromResult<IReadOnlyCollection<FolderDto>>(result);
    }

    public Task<FolderDto> Get(Guid _, Guid folderId)
    {
        var folder = _entities.Single(f => f.Id == folderId);

        return Task.FromResult(new FolderDto(folder.Id, folder.ParentId, folder.Name.Value));
    }

    public Task Remove(Guid _, Guid folderId)
    {
        var folder = _entities.Single(f => f.Id == folderId);
        _entities.Remove(folder);

        return Task.CompletedTask;
    }

    public Task Update(Guid _, Guid folderId, string name)
    {
        var folder = _entities.Single(f => f.Id == folderId);
        _entities.Remove(folder);
        _entities.Add(new Folder(folderId, folder.ParentId, Name.Create(name)));

        return Task.CompletedTask;
    }
}
