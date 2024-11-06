namespace Fotos.Tests.Backend.PhotoFolders;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
internal sealed class CreateFolderWrongTheoryData : TheoryData<string>
{
    public CreateFolderWrongTheoryData()
    {
        Add(null!);
        Add(string.Empty);
        Add(" ");
        Add("""
{
    "parentId":"6c14003f-ea36-4c48-9e15-775ef47c6ccc",
    "name":""
}
""");
        Add("""
{
    "parentId":"6c14003f-ea36-4c48-9e15-775ef47c6ccc",
    "name":" "
}
""");
        Add("""
{
    "parentId":"",
    "name":"my folder"
}
""");
        Add("""
{
    "parentId":" ",
    "name":"my folder"
}
""");
        Add("""
{
    "parentId":"not a guid",
    "name":"my folder"
}
""");
        Add("""
{
    "parentId":"not a guid"
}
""");
        Add("""
{
    "name":"my folder"
}
""");
        Add("""
{
    "parentId":"6c14003f-ea36-4c48-9e15-775ef47c6ccc",
    "name": null
}
""");
        Add("""
{
    "parentId": null,
    "name":"my folder"
}
""");
        Add("""
{
    "parentId":"00000000-0000-0000-0000-000000000000",
    "name":"my folder"
}
""");
    }
}