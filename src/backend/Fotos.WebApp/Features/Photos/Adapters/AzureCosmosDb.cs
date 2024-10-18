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

        await _photoContainer.CreateItemAsync(cosmosPhoto, ToPartitionKey(photo.Id));
    }

    public async Task<IReadOnlyCollection<PhotoEntity>> ListPhotos(AlbumId albumId)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.folderId = @folderId AND c.albumId = @albumId")
            .WithParameter("@folderId", albumId.FolderId)
            .WithParameter("@albumId", albumId.Id);

        var photos = new List<PhotoEntity>();
        var iterator = _photoContainer.GetItemQueryIterator<CosmosPhoto>(query, requestOptions: new() { PartitionKey = ToPartitionKey(albumId) });

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();

            photos.AddRange(response.Select(x => x.ToPhotoEntity()));
        }

        return photos;
    }

    public async Task RemovePhoto(PhotoId photoId)
    {
        await _photoContainer.DeleteItemAsync<CosmosPhoto>(photoId.Id.ToString(), ToPartitionKey(photoId));
    }

    public async Task<PhotoEntity> GetPhoto(PhotoId photoId)
    {
        var response = await _photoContainer.ReadItemAsync<CosmosPhoto>(photoId.Id.ToString(), ToPartitionKey(photoId));

        return response.Resource.ToPhotoEntity();
    }

    private static PartitionKey ToPartitionKey(PhotoId photoId) => new PartitionKeyBuilder()
            .Add(photoId.FolderId.ToString())
            .Add(photoId.AlbumId.ToString())
            .Build();

    private static PartitionKey ToPartitionKey(AlbumId albumId) => new PartitionKeyBuilder()
            .Add(albumId.FolderId.ToString())
            .Add(albumId.Id.ToString())
            .Build();

    private sealed record CosmosPhoto(Guid Id, Guid FolderId, Guid AlbumId, string Title, ExifMetadata? Metadata)
    {
        public PhotoEntity ToPhotoEntity() => new(
                new PhotoId(FolderId, AlbumId, Id),
                Title,
                Metadata);
    }
}
