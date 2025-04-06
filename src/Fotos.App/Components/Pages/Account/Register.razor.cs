using Fotos.App.Application.User;
using Fotos.App.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Fotos.App.Components.Pages.Account;
public partial class Register
{
    [Inject]
    internal AddUserBusiness AddUser { get; set; } = default!;

    private string _givenName = string.Empty;
    private string _provider = string.Empty;
    private string _providerUserId = string.Empty;
    private AuthenticationState _authState = default!;

    protected override async Task OnInitializedAsync()
    {
        _authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        _givenName = _authState.User.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
        _provider = _authState.User.Identity?.AuthenticationType ?? string.Empty;
        _providerUserId = _authState.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    private async Task RegisterUser()
    {
        await AddUser.Process(FotoUserId.Create(_provider, _providerUserId), _givenName);

        NavigationManager.NavigateTo("/account/login-callback", true);
    }
}