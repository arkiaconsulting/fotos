namespace Fotos.WebApp.Tests.Features.Account;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
internal sealed class AddUserWrongTheoryData : TheoryData<string, string>
{
    public AddUserWrongTheoryData()
    {
        Add("null body", null!);
        Add("emtpy body", string.Empty);
        Add("whitespace body", " ");

        // Provider
        Add("empty provider", """
{
    "provider":"",
    "providerUserId": "any",
    "givenName": "any"
}
""");
        Add("whitespace provider", """
{
    "provider":" ",
    "providerUserId": "any",
    "givenName": "any"
}
""");
        Add("null provider", """
{
    "provider": null,
    "providerUserId": "any",
    "givenName": "any"
}
""");

        // ProviderUserId
        Add("empty provider user ID", """
{
    "provider":"any",
    "providerUserId": "",
    "givenName": "any"
}
""");
        Add("whitespace provider user ID", """
{
    "provider":"any",
    "providerUserId": " ",
    "givenName": "any"
}
""");
        Add("null provider user ID", """
{
    "provider": "any",
    "providerUserId": null,
    "givenName": "any"
}
""");

        // GivenName
        Add("empty given name", """
{
    "provider":"any",
    "providerUserId": "any",
    "givenName": ""
}
""");
        Add("whitespace given name", """
{
    "provider":"any",
    "providerUserId": "any",
    "givenName": " "
}
""");
        Add("null given name", """
{
    "provider": "any",
    "providerUserId": "any",
    "givenName": null
}
""");
    }
}
