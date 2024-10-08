namespace Fotos.WebApp.Tests.Features.PhotoAlbums;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
internal sealed class CreateAlbumWrongTheoryData : TheoryData<string>
{
    public CreateAlbumWrongTheoryData()
    {
        Add(null!);
        Add(string.Empty);
        Add(" ");
        Add("""
{
    "folderId":"6c14003f-ea36-4c48-9e15-775ef47c6ccc",
    "name":""
}
""");
        Add("""
{
    "folderId":"6c14003f-ea36-4c48-9e15-775ef47c6ccc",
    "name":" "
}
""");
        Add("""
{
    "folderId":"",
    "name":"my folder"
}
""");
        Add("""
{
    "folderId":" ",
    "name":"my folder"
}
""");
        Add("""
{
    "folderId":"not a guid",
    "name":"my folder"
}
""");
        Add("""
{
    "folderId":"not a guid"
}
""");
        Add("""
{
    "name":"my folder"
}
""");
        Add("""
{
    "folderId":"6c14003f-ea36-4c48-9e15-775ef47c6ccc",
    "name": null
}
""");
        Add("""
{
    "folderId": null,
    "name":"my folder"
}
""");
    }
}