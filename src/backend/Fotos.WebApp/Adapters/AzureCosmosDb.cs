using Fotos.WebApp.Features.PhotoAlbums;
using Fotos.WebApp.Features.PhotoFolders;
using Fotos.WebApp.Types;
using Microsoft.Azure.Cosmos;

namespace Fotos.WebApp.Adapters;

internal sealed class AzureCosmosDb
{
    private readonly Container _photoContainer;
    private readonly Container _folderContainer;
    private readonly Container _albumContainer;

    public AzureCosmosDb(CosmosClient client, IConfiguration configuration)
    {
        _photoContainer = client.GetDatabase(configuration["CosmosDb:DatabaseId"])
            .GetContainer(configuration["CosmosDb:ContainerId"]);
        _folderContainer = client.GetDatabase(configuration["CosmosDb:DatabaseId"])
            .GetContainer(configuration["CosmosDb:FoldersContainerId"]);
        _albumContainer = client.GetDatabase(configuration["CosmosDb:DatabaseId"])
            .GetContainer(configuration["CosmosDb:AlbumsContainerId"]);
    }
    public async Task SavePhoto(PhotoEntity photo)
    {
        var cosmosPhoto = new CosmosPhoto(photo.Id.Id, photo.Id.FolderId, photo.Id.AlbumId, photo.Title, photo.Metadata);

        await _photoContainer.UpsertItemAsync(cosmosPhoto, ToPartitionKey(photo.Id));
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

    public async Task StoreFolder(Folder folder)
    {
        var cosmosFolder = new CosmosFolder(folder.Id, folder.ParentId, folder.Name.Value);

        await _folderContainer.CreateItemAsync(cosmosFolder, new(folder.ParentId.ToString()));
    }

    public async Task<IReadOnlyCollection<Folder>> GetFolders(Guid parentId)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.parentId = @parentId")
            .WithParameter("@parentId", parentId);
        var folders = new List<Folder>();

        var iterator = _folderContainer.GetItemQueryIterator<CosmosFolder>(query, requestOptions: new() { PartitionKey = new PartitionKey(parentId.ToString()) });
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            folders.AddRange(response.Select(x => new Folder(x.Id, x.ParentId, new(x.Name))));
        }

        return folders;
    }

    public async Task<Folder> GetFolder(Guid parentId, Guid folderId)
    {
        var response = await _folderContainer.ReadItemAsync<CosmosFolder>(folderId.ToString(), new PartitionKey(parentId.ToString()));

        return new Folder(response.Resource.Id, response.Resource.ParentId, new(response.Resource.Name));
    }

    public async Task RemoveFolder(Guid parentId, Guid folderId)
    {
        await _folderContainer.DeleteItemAsync<CosmosFolder>(folderId.ToString(), new PartitionKey(parentId.ToString()));
    }

    public async Task<IReadOnlyCollection<Album>> GetAlbums(Guid folderId)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.folderId = @folderId")
            .WithParameter("@folderId", folderId);
        var albums = new List<Album>();

        var iterator = _albumContainer.GetItemQueryIterator<CosmosAlbum>(query, requestOptions: new() { PartitionKey = new PartitionKey(folderId.ToString()) });
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            albums.AddRange(response.Select(x => new Album(x.Id, x.FolderId, new(x.Name))));
        }

        return albums;
    }

    public async Task StoreAlbum(Album album)
    {
        var cosmosAlbum = new CosmosAlbum(album.Id, album.FolderId, album.Name.Value);

        await _albumContainer.CreateItemAsync(cosmosAlbum, new(album.FolderId.ToString()));
    }

    public async Task<Album> GetAlbum(AlbumId albumId)
    {
        var response = await _albumContainer.ReadItemAsync<CosmosAlbum>(albumId.Id.ToString(), new PartitionKey(albumId.FolderId.ToString()));

        return new Album(response.Resource.Id, response.Resource.FolderId, new(response.Resource.Name));
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

    private sealed record CosmosFolder(Guid Id, Guid ParentId, string Name);

    private sealed record CosmosAlbum(Guid Id, Guid FolderId, string Name);
}
