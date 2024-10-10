namespace Fotos.WebApp.Tests.Features.PhotoAlbums;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
internal sealed class CreateAlbumWrongTheoryData : TheoryData<string, string, string>
{
    public CreateAlbumWrongTheoryData()
    {
        Add("null body", null!, Guid.NewGuid().ToString());
        Add("emtpy body", string.Empty, Guid.NewGuid().ToString());
        Add("whitespace body", " ", Guid.NewGuid().ToString());
        Add("empty name", """
{
    "name":""
}
""", Guid.NewGuid().ToString());
        Add("whitespace name", """
{
    "name":" "
}
""", Guid.NewGuid().ToString());
        Add("null name", """
{
    "name": null
}
""", Guid.NewGuid().ToString());
    }
}