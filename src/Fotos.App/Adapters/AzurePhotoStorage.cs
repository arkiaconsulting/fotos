using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Fotos.App.Domain;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Fotos.App.Adapters;

internal sealed class AzurePhotoStorage
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IMemoryCache _cache;
    private readonly string? _mainContainer;
    private readonly ActivitySource _activitySource;
    private const string CacheKey = "UserDelegationKey";

    public AzurePhotoStorage(
        BlobServiceClient blobServiceClient,
        IConfiguration configuration,
        IMemoryCache cache,
        InstrumentationConfig instrumentation)
    {
        _blobServiceClient = blobServiceClient;
        _cache = cache;
        _mainContainer = configuration[$"{Constants.BlobServiceClientName}:PhotosContainer"];
        _activitySource = instrumentation.ActivitySource;
    }

    public async Task AddOriginalPhoto(PhotoId photoId, Stream photo, string contentType)
    {
        using var activity = _activitySource.StartActivity("store photo original in storage");

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

        activity?.AddEvent(new ActivityEvent("photo original stored", tags: [new("blobName", blobClient.Name)]));
    }

    public async Task<Uri> GetOriginalUri(PhotoId photoId)
    {
        using var activity = _activitySource.StartActivity("get original photo uri from storage");

        var blobName = ComputeOriginalName(photoId);
        var uri = await GetAuthorizedUri(blobName)!;

        activity?.AddEvent(new ActivityEvent("original photo uri retrieved", tags: [new("blobName", blobName)]));

        return uri;
    }

    public async Task<Uri> GetThumbnailUri(PhotoId photoId)
    {
        using var activity = _activitySource.StartActivity("get thumbnail photo uri from storage");

        var blobName = ComputeThumbnailName(photoId);
        var uri = await GetAuthorizedUri(blobName)!;

        activity?.AddEvent(new ActivityEvent("thumbnail photo uri retrieved", tags: [new("blobName", blobName)]));

        return uri;
    }

    public async Task<PhotoBinary> ReadOriginalPhoto(PhotoId photoId)
    {
        using var activity = _activitySource.StartActivity("read photo original from storage");

        var container = _blobServiceClient.GetBlobContainerClient(_mainContainer);
        var blobClient = container.GetBlobClient(ComputeOriginalName(photoId));
        var blobDownloadInfo = await blobClient.DownloadContentAsync();

        activity?.AddEvent(new ActivityEvent("photo original read", tags: [new("blobName", blobClient.Name), new("size", blobDownloadInfo.Value.Details.ContentLength)]));

        return new(blobDownloadInfo.Value.Content.ToStream(), blobDownloadInfo.Value.Details.ContentType);
    }

    public async Task AddPhotoToThumbnailStorage(PhotoId photoId, PhotoBinary photo)
    {
        using var activity = _activitySource.StartActivity("store photo thumbnail in storage");

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

        activity?.AddEvent(new ActivityEvent("photo thumbnail stored", tags: [new("blobName", blobClient.Name)]));
    }

    public async Task RemovePhotoOriginal(PhotoId photoId)
    {
        using var activity = _activitySource.StartActivity("remove photo original from storage");

        var blobName = ComputeOriginalName(photoId);
        await RemoveBlob(blobName);

        activity?.AddEvent(new ActivityEvent("photo original removed", tags: [new("blobName", blobName)]));
    }

    public async Task RemovePhotoThumbnail(PhotoId photoId)
    {
        using var activity = _activitySource.StartActivity("remove photo thumbnail from storage");

        var blobName = ComputeThumbnailName(photoId);
        await RemoveBlob(ComputeThumbnailName(photoId));

        activity?.AddEvent(new ActivityEvent("photo thumbnail removed", tags: [new("blobName", blobName)]));
    }

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
