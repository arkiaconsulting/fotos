namespace Fotos.Tests.Backend.Account;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
internal sealed class AddUserWrongTheoryData : TheoryData<string, string>
{
    public AddUserWrongTheoryData()
    {
        Add("null body", null!);
        Add("emtpy body", string.Empty);
        Add("whitespace body", " ");

        // GivenName
        Add("empty given name", """
{
    "givenName": ""
}
""");
        Add("whitespace given name", """
{
    "givenName": " "
}
""");
        Add("null given name", """
{
    "givenName": null
}
""");
    }
}
