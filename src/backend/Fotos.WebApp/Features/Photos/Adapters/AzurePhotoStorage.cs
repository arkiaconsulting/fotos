using Azure.Storage.Blobs;
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

        await container.UploadBlobAsync(ComputeOriginalName(photoId), photo);
    }

    private static string ComputeOriginalName(PhotoId photoId) => $"{photoId.Id}.original";
}
