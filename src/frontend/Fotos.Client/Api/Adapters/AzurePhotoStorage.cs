using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Fotos.Client.Api.Photos;
using Microsoft.Extensions.Caching.Memory;

namespace Fotos.Client.Api.Adapters;

internal sealed class AzurePhotoStorage
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IMemoryCache _cache;
    private readonly string? _mainContainer;
    private const string CacheKey = "UserDelegationKey";
    public AzurePhotoStorage(
        BlobServiceClient blobServiceClient,
        IConfiguration configuration,
        IMemoryCache cache)
    {
        _blobServiceClient = blobServiceClient;
        _cache = cache;
        _mainContainer = configuration[$"{Constants.BlobServiceClientName}:PhotosContainer"];
    }

    public async Task AddOriginalPhoto(PhotoId photoId, Stream photo, string contentType)
    {
        var container = _blobServiceClient.GetBlobContainerClient(_mainContainer);
        var blobClient = container.GetBlobClient(ComputeOriginalName(photoId));

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType,
            },
        };

        await blobClient.UploadAsync(photo, options);
    }

    public async Task<Uri> GetOriginalUri(PhotoId photoId) =>
        await GetAuthorizedUri(ComputeOriginalName(photoId))!;

    public async Task<Uri> GetThumbnailUri(PhotoId photoId) =>
        await GetAuthorizedUri(ComputeThumbnailName(photoId))!;

    public async Task<PhotoBinary> ReadOriginalPhoto(PhotoId photoId)
    {
        var container = _blobServiceClient.GetBlobContainerClient(_mainContainer);
        var blobClient = container.GetBlobClient(ComputeOriginalName(photoId));
        var blobDownloadInfo = await blobClient.DownloadContentAsync();

        return new(blobDownloadInfo.Value.Content.ToStream(), blobDownloadInfo.Value.Details.ContentType);
    }

    public async Task AddPhotoToThumbnailStorage(PhotoId photoId, PhotoBinary photo)
    {
        var container = _blobServiceClient.GetBlobContainerClient(_mainContainer);
        var blobClient = container.GetBlobClient(ComputeThumbnailName(photoId));

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = photo.MimeType,
            },
        };

        await blobClient.UploadAsync(photo.Content, options);
    }

    public async Task RemovePhotoOriginal(PhotoId photoId) => await RemoveBlob(ComputeOriginalName(photoId));

    public async Task RemovePhotoThumbnail(PhotoId photoId) => await RemoveBlob(ComputeThumbnailName(photoId));

    private async Task RemoveBlob(string blobName)
    {
        var container = _blobServiceClient.GetBlobContainerClient(_mainContainer);
        var blobClient = container.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }

    private async Task<Uri> GetAuthorizedUri(string blobName)
    {
        var container = _blobServiceClient.GetBlobContainerClient(_mainContainer);
        var blobClient = container.GetBlobClient(blobName);
        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddSeconds(30),
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        if (!blobClient.CanGenerateSasUri)
        {
            var expiryTime = DateTimeOffset.UtcNow.AddMinutes(5);
            var userDelegationKey = await _cache.GetOrCreateAsync(CacheKey, async entry =>
            {
                entry.AbsoluteExpiration = expiryTime.AddSeconds(-30);

                return await _blobServiceClient.GetUserDelegationKeyAsync(null, expiryTime, CancellationToken.None);
            });

            return new BlobUriBuilder(blobClient.Uri)
            {
                Sas = sasBuilder.ToSasQueryParameters(userDelegationKey!.Value, _blobServiceClient.AccountName),
            }.ToUri();
        }

        return blobClient.GenerateSasUri(sasBuilder);
    }

    private static string ComputeOriginalName(PhotoId photoId) => $"{photoId.Id}.original";

    private static string ComputeThumbnailName(PhotoId photoId) => $"{photoId.Id}.thumbnail";
}
