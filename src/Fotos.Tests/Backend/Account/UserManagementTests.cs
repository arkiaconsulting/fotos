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
        var client = _fotoApi.CreateClient();

        using var response = await client.AddUser(givenName);

        response.Should().Be204NoContent();
    }

    [Theory(DisplayName = "Creating the authenticated user with an invalid payload should fail"), ClassData(typeof(AddUserWrongTheoryData))]
    internal async Task Test02(string _, string body)
    {
        var client = _fotoApi.CreateClient();

        using var response = await client.AddUserWithBody(body);

        response.Should().Be400BadRequest();
        response.Should().MatchInContent("*https://tools.ietf.org/html/rfc9110#section-15.5.1*");
    }

    [Theory(DisplayName = "Getting the authenticated user details should pass"), AutoData]
    internal async Task Test03(string givenName)
    {
        var client = _fotoApi.CreateClient();
        using var _ = await client.AddUser(givenName);

        using var userResponse = await client.GetMe();

        userResponse.Should().Be200Ok();
        userResponse.Should().MatchInContent($"*{givenName}*");
    }
}
