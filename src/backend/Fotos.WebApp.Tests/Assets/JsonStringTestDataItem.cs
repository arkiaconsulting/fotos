using Xunit.Abstractions;

namespace Fotos.WebApp.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
internal sealed class JsonStringTestDataItem : IXunitSerializable
{
    private readonly string _title;

    public JsonStringTestDataItem() => _title = "no title";

    public JsonStringTestDataItem(string title, string json)
    {
        _title = title;
        Json = json;
    }

    public string Json { get; set; } = default!;

    public void Deserialize(IXunitSerializationInfo info) => Json = info.GetValue<string>(nameof(Json));

    public void Serialize(IXunitSerializationInfo info) => info.AddValue(nameof(Json), Json);

    public override string ToString() => _title;
}