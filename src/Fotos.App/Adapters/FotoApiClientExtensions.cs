using Fotos.App.Api.Account;

namespace Fotos.App.Adapters;

internal static class FotoApiClientExtensions
{
    public static Task<FotoUserDto> GetMe(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<FotoUserDto>("api/users/me");
}
