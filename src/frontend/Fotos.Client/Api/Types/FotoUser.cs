using Fotos.Client.Api.Shared;

namespace Fotos.Client.Api.Types;

public readonly record struct FotoUser(FotoUserId Id, Name GivenName, Guid RootFolderId);

public readonly record struct FotoUserId(string Value)
{
    public static FotoUserId Create(string provider, string providerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(provider, nameof(provider));
        ArgumentException.ThrowIfNullOrWhiteSpace(providerId, nameof(providerId));

        return new FotoUserId($"{provider.ToLowerInvariant()}-{providerId}");
    }
}