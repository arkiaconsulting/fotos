using Fotos.WebApp.Types;
using Microsoft.Azure.Cosmos;

namespace Fotos.WebApp.Features.Photos.Adapters;

internal sealed class AzureCosmosDb
{
    private readonly Container _photoContainer;

    public AzureCosmosDb(CosmosClient client, IConfiguration configuration)
    {
        _photoContainer = client.GetDatabase(configuration["CosmosDb:DatabaseId"])
            .GetContainer(configuration["CosmosDb:ContainerId"]);
    }
    public async Task SavePhoto(PhotoEntity photo)
    {
        var cosmosPhoto = new CosmosPhoto(photo.Id.Id, photo.Id.FolderId, photo.Id.AlbumId, photo.Title, photo.Metadata);

        await _photoContainer.CreateItemAsync(cosmosPhoto);
    }

    private sealed record CosmosPhoto(Guid Id, Guid FolderId, Guid AlbumId, string Title, ExifMetadata? Metadata);
}
