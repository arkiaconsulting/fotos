using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Fotos.App.Api.Account;

internal static class Helpers
{
    public static ChallengeHttpResult Challenge(string provider, string authenticationScheme, string returnUrl)
    {
        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = "/account/login-callback",
            Items =
            {
                { "scheme", provider },
                { "returnUrl", returnUrl }
            }
        };

        return TypedResults.Challenge(authenticationProperties, [authenticationScheme]);
    }

    public static string ComputeSafeReturnUrl(string pathBase, string? returnUrl)
    {
        var basePath = string.IsNullOrWhiteSpace(pathBase) ? "/" : pathBase;

        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return basePath;
        }
        else if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
        {
            return new Uri(returnUrl, UriKind.Absolute).PathAndQuery;
        }
        else if (returnUrl[0] != '/')
        {
            return basePath + returnUrl;
        }

        return returnUrl;
    }
}
