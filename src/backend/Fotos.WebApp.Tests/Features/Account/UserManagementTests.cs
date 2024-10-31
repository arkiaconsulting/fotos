using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.WebApp.Tests.Assets;

namespace Fotos.WebApp.Tests.Features.Account;

[Trait("Category", "Unit")]
public sealed class UserManagementTests : IClassFixture<FotoApi>
{
    private readonly FotoApi _fotoApi;

    public UserManagementTests(FotoApi fotoApi) => _fotoApi = fotoApi;

    [Theory(DisplayName = "Adding a new user should pass"), AutoData]
    public async Task Test01(string provider, string providerId, string givenName)
    {
        var client = _fotoApi.CreateClient();

        using var response = await client.AddUser(provider, providerId, givenName);

        response.Should().Be204NoContent();
    }

    [Theory(DisplayName = "Adding a new user with an invalid payload should fail"), ClassData(typeof(AddUserWrongTheoryData))]
    internal async Task Test02(string _, string body)
    {
        var client = _fotoApi.CreateClient();

        using var response = await client.AddUserWithBody(body);

        response.Should().Be400BadRequest();
        response.Should().MatchInContent("*https://tools.ietf.org/html/rfc9110#section-15.5.1*");
    }
}
