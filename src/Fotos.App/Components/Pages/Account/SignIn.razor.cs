using Microsoft.AspNetCore.Components;

namespace Fotos.App.Components.Pages.Account;
public partial class SignIn
{
    [Parameter]
    public Uri ReturnUrl { get; set; } = new("/", UriKind.Relative);
}