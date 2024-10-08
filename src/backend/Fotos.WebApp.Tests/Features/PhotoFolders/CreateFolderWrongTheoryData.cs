﻿namespace Fotos.WebApp.Tests.Features.PhotoFolders;

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
    "parentFolderId":"6c14003f-ea36-4c48-9e15-775ef47c6ccc",
    "name":""
}
""");
        Add("""
{
    "parentFolderId":"6c14003f-ea36-4c48-9e15-775ef47c6ccc",
    "name":" "
}
""");
        Add("""
{
    "parentFolderId":"",
    "name":"my folder"
}
""");
        Add("""
{
    "parentFolderId":" ",
    "name":"my folder"
}
""");
        Add("""
{
    "parentFolderId":"not a guid",
    "name":"my folder"
}
""");
        Add("""
{
    "parentFolderId":"not a guid"
}
""");
        Add("""
{
    "name":"my folder"
}
""");
        Add("""
{
    "parentFolderId":"6c14003f-ea36-4c48-9e15-775ef47c6ccc",
    "name": null
}
""");
        Add("""
{
    "parentFolderId": null,
    "name":"my folder"
}
""");
        Add("""
{
    "parentFolderId":"00000000-0000-0000-0000-000000000000",
    "name":"my folder"
}
""");
    }
}