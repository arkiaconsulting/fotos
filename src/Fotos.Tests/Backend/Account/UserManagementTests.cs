using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.Tests.Backend.Assets;

namespace Fotos.Tests.Backend.Account;

[Trait("Category", "Unit")]
public sealed class UserManagementTests : IClassFixture<FotoApi>
{
    private readonly FotoApi _fotoApi;

    public UserManagementTests(FotoApi fotoApi) => _fotoApi = fotoApi;

    [Theory(DisplayName = "Creating the authenticated user should pass"), AutoData]
    public async Task Test01(string givenName)
    {
        var client = _fotoApi.CreateAuthenticatedClient();

        using var response = await client.AddUser(givenName);

        response.Should().Be204NoContent();
    }

    [Theory(DisplayName = "Creating the authenticated user with an invalid payload should fail"), ClassData(typeof(AddUserWrongTheoryData))]
    internal async Task Test02(string _, string body)
    {
        var client = _fotoApi.CreateAuthenticatedClient();

        using var response = await client.AddUserWithBody(body);

        response.Should().Be400BadRequest();
        response.Should().MatchInContent("*https://tools.ietf.org/html/rfc9110#section-15.5.1*");
    }
}
