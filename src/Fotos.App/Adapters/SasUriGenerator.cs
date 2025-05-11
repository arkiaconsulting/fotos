using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Caching.Memory;

namespace Fotos.App.Adapters;

internal sealed class SasUriGenerator
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IMemoryCache _cache;

    public SasUriGenerator(
        BlobServiceClient blobServiceClient,
        IMemoryCache cache)
    {
        _blobServiceClient = blobServiceClient;
        _cache = cache;
    }

    public async Task<Uri> GenerateSasUri(BlobClient blobClient, BlobSasBuilder sasBuilder)
    {
        var expiryTime = DateTimeOffset.UtcNow.AddMinutes(5);
        var userDelegationKey = await _cache.GetOrCreateAsync("UserDelegationKey", async entry =>
        {
            entry.AbsoluteExpiration = expiryTime.AddSeconds(-30);

            return await _blobServiceClient.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow.AddMinutes(-5), expiryTime, CancellationToken.None);
        }) ?? throw new InvalidOperationException("No response received while getting the storage user delegation key");

        return new BlobUriBuilder(blobClient.Uri)
        {
            Sas = sasBuilder.ToSasQueryParameters(userDelegationKey, _blobServiceClient.AccountName),
        }.ToUri();
    }
}