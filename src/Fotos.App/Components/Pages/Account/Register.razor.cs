using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Fotos.App.Components.Pages.Account;
public partial class Register
{
    private string _givenName = string.Empty;
    private AuthenticationState _authState = default!;

    protected override async Task OnInitializedAsync()
    {
        _authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        _givenName = _authState.User.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
    }

    private async Task RegisterUser()
    {
        await SaveUser(_givenName);

        NavigationManager.NavigateTo("/account/login-callback", true);
    }
}