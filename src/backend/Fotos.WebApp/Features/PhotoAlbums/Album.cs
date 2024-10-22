using Fotos.WebApp.Features.Shared;

namespace Fotos.WebApp.Features.PhotoAlbums;

internal readonly record struct Album(Guid Id, Guid FolderId, Name Name)
{
    public static Album Create(Guid id, Guid folderId, string name) => new(id, folderId, new(name));
}
