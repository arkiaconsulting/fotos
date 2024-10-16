using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Fotos.WebApp.Types;

namespace Fotos.WebApp.Features.Photos.Adapters;

internal sealed class AzurePhotoStorage
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string? _mainContainer;

    public AzurePhotoStorage(
        BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;
        _mainContainer = configuration[$"{Constants.BlobServiceClientName}:PhotosContainer"];
    }

    public async Task AddOriginalPhoto(PhotoId photoId, Stream photo)
    {
        var container = _blobServiceClient.GetBlobContainerClient(_mainContainer);
        var blobClient = container.GetBlobClient(ComputeOriginalName(photoId));

        await blobClient.UploadAsync(photo);
    }

    public async Task<Uri> GetOriginalUri(PhotoId photoId)
    {
        await Task.CompletedTask;

        var container = _blobServiceClient.GetBlobContainerClient(_mainContainer);

        var blobClient = container.GetBlobClient(ComputeOriginalName(photoId));

        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddSeconds(30),
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return blobClient.GenerateSasUri(sasBuilder);
    }

    private static string ComputeOriginalName(PhotoId photoId) => $"{photoId.Id}.original";
}
