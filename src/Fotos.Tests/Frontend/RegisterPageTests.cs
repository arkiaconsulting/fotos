using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.App.Components.Pages.Account;
using Fotos.Tests.Frontend.Assets;
using System.Security.Claims;

namespace Fotos.Tests.Frontend;

[Trait("Category", "Unit")]
[Trait("Category", "Blazor")]
public sealed class RegisterPageTests : IDisposable
{
    private readonly FotosTestContext _testContext;

    public RegisterPageTests() => _testContext = new FotosTestContext();

    [Theory(DisplayName = "Register page should display a form to create a new user"), AutoData]
    public void Test01(string userProviderId, string userName)
    {
        _testContext.AuthContext.SetAuthorized(userProviderId);
        _testContext.AuthContext.SetClaims([new Claim(ClaimTypes.GivenName, userName)]);
        var register = _testContext.RenderComponent<Register>();

        register.WaitForElement("#given-name").GetAttribute("value").Should().Be(userName);
        register.WaitForElement("button#register").Should().NotBeNull();
    }

    #region IDisposable

    public void Dispose() => _testContext.Dispose();

    #endregion
}
