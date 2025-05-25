using Fotos.Application;
using Fotos.Application.User;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Fotos.App.Components.Pages.Account;

public partial class Register
{
    [CascadingParameter]
    public ProcessError? ProcessError { get; set; }

    [Inject]
    internal ISender Sender { get; set; } = default!;

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
        using var activity = DiagnosticConfig.StartUserActivity("Register new user");

        try
        {
            var command = new AddUserCommand(FotoUserId.Create(_provider, _providerUserId), _givenName);

            var result = await Sender.Send(command);

            NavigationManager.NavigateTo("/account/login-callback", true);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, "Unable to register new user");
            activity?.AddException(ex);
            ProcessError?.LogError(ex);
        }
    }
}