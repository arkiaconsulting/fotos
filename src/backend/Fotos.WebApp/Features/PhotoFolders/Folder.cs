using Fotos.WebApp.Features.Shared;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fotos.WebApp.Features.PhotoFolders;

internal readonly record struct Folder(Guid Id, Guid ParentId, [property: JsonConverter(typeof(NameJsonConverter))] Name Name)
{
    public static Folder Create(Guid id, Guid parentId, string name)
    {
        return new Folder(id, parentId, Name.Create(name));
    }
}

internal sealed class NameJsonConverter : JsonConverter<Name>
{
    public override Name Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, Name value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}