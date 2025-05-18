using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Fotos.App.Domain;
using Microsoft.Extensions.Azure;
using System.Diagnostics;

namespace Fotos.App.Adapters.Storage;

internal sealed class AzurePhotoStorage
{
    private readonly BlobContainerClient _container;
    private readonly SasUriGenerator _sasUriGenerator;

    public AzurePhotoStorage(
        IAzureClientFactory<BlobContainerClient> azureClientFactory,
        SasUriGenerator sasUriGenerator)
    {
        _container = azureClientFactory.CreateClient(Constants.PhotosBlobContainer);
        _sasUriGenerator = sasUriGenerator;
    }

    public async Task AddOriginalPhoto(PhotoId photoId, Stream photo, string contentType)
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity("store photo original in storage");

        var blobClient = _container.GetBlobClient(ComputeOriginalName(photoId));

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType,
            },
        };

        try
        {
            await blobClient.UploadAsync(photo, options);

            activity?.AddEvent(new ActivityEvent("photo original stored", tags: [new("blobName", blobClient.Name)]));
        }
        catch (RequestFailedException ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Unable to upload photo original.");
            activity?.AddException(ex);

            throw;
        }
    }

    public async Task<Uri> GetOriginalUri(PhotoId photoId)
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity("get original photo uri from storage");

        var blobName = ComputeOriginalName(photoId);

        try
        {
            var uri = await GetAuthorizedUri(blobName)!;

            activity?.AddEvent(new ActivityEvent("original photo uri retrieved", tags: [new("blobName", blobName)]));

            return uri;
        }
        catch (RequestFailedException ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Unable to get original photo uri.");
            activity?.AddException(ex);

            throw;
        }
    }

    public async Task<Uri> GetThumbnailUri(PhotoId photoId)
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity("get thumbnail photo uri from storage");

        var blobName = ComputeThumbnailName(photoId);
        try
        {
            var uri = await GetAuthorizedUri(blobName)!;

            activity?.AddEvent(new ActivityEvent("thumbnail photo uri retrieved", tags: [new("blobName", blobName)]));

            return uri;
        }
        catch (RequestFailedException ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Unable to get thumbnail photo uri.");
            activity?.AddException(ex);

            throw;
        }
    }

    public async Task<PhotoBinary> ReadOriginalPhoto(PhotoId photoId)
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity("read photo original from storage");

        var blobClient = _container.GetBlobClient(ComputeOriginalName(photoId));
        try
        {
            var blobDownloadInfo = await blobClient.DownloadContentAsync();

            activity?.AddEvent(new ActivityEvent("photo original read", tags: [new("blobName", blobClient.Name), new("size", blobDownloadInfo.Value.Details.ContentLength)]));

            return new(blobDownloadInfo.Value.Content.ToStream(), blobDownloadInfo.Value.Details.ContentType);
        }
        catch (RequestFailedException ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Unable to read photo original.");
            activity?.AddException(ex);

            throw;
        }
    }

    public async Task AddPhotoToThumbnailStorage(PhotoId photoId, PhotoBinary photo)
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity("store photo thumbnail in storage");

        var blobClient = _container.GetBlobClient(ComputeThumbnailName(photoId));

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = photo.MimeType,
            },
        };

        try
        {
            await blobClient.UploadAsync(photo.Content, options);

            activity?.AddEvent(new ActivityEvent("photo thumbnail stored", tags: [new("blobName", blobClient.Name)]));
        }
        catch (RequestFailedException ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Unable to upload photo thumbnail.");
            activity?.AddException(ex);

            throw;
        }
    }

    public async Task RemovePhotoOriginal(PhotoId photoId)
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity("remove photo original from storage");

        var blobName = ComputeOriginalName(photoId);
        try
        {
            await RemoveBlob(blobName);

            activity?.AddEvent(new ActivityEvent("photo original removed", tags: [new("blobName", blobName)]));
        }
        catch (RequestFailedException ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Unable to remove photo original.");
            activity?.AddException(ex);

            throw;
        }
    }

    public async Task RemovePhotoThumbnail(PhotoId photoId)
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity("remove photo thumbnail from storage");

        var blobName = ComputeThumbnailName(photoId);
        try
        {
            await RemoveBlob(ComputeThumbnailName(photoId));

            activity?.AddEvent(new ActivityEvent("photo thumbnail removed", tags: [new("blobName", blobName)]));
        }
        catch (RequestFailedException ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Unable to remove photo thumbnail.");
            activity?.AddException(ex);

            throw;
        }
    }

    #region Private

    private async Task RemoveBlob(string blobName)
    {
        var blobClient = _container.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }

    private async Task<Uri> GetAuthorizedUri(string blobName)
    {
        var blobClient = _container.GetBlobClient(blobName);
        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddSeconds(30),
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return await _sasUriGenerator.GenerateSasUri(blobClient, sasBuilder);
    }

    private static string ComputeOriginalName(PhotoId photoId) => $"{photoId.Id}.original";

    private static string ComputeThumbnailName(PhotoId photoId) => $"{photoId.Id}.thumbnail";

    #endregion Private
}
