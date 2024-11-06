namespace Fotos.Tests.Backend.PhotoAlbums;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
internal sealed class CreateAlbumWrongTheoryData : TheoryData<string, string>
{
    public CreateAlbumWrongTheoryData()
    {
        Add("null body", null!);
        Add("emtpy body", string.Empty);
        Add("whitespace body", " ");
        Add("empty name", """
{
    "name":""
}
""");
        Add("whitespace name", """
{
    "name":" "
}
""");
        Add("null name", """
{
    "name": null
}
""");
    }
}