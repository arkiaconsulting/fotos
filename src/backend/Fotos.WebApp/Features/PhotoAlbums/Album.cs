using Fotos.WebApp.Features.PhotoFolders;
using Fotos.WebApp.Features.Shared;
using System.Text.Json.Serialization;

namespace Fotos.WebApp.Features.PhotoAlbums;

internal readonly record struct Album(Guid Id, Guid FolderId, [property: JsonConverter(typeof(NameJsonConverter))] Name Name)
{
    public static Album Create(Guid id, Guid folderId, string name) => new(id, folderId, new(name));
}
