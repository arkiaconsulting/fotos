using Fotos.App.Adapters;
using Fotos.App.Api.PhotoAlbums;
using Fotos.App.Api.Photos;
using Fotos.App.Api.Shared;
using Fotos.App.Api.Types;
using Fotos.App.Features.PhotoAlbums;
using Fotos.App.Features.Photos;
using Microsoft.Azure.Cosmos;
using System.Diagnostics;

namespace Fotos.App.Api.Adapters;

internal sealed class AzureCosmosDb
{
    private readonly Container _photoContainer;
    private readonly Container _folderContainer;
    private readonly Container _albumContainer;
    private readonly Container _sessionDataContainer;
    private readonly Container _userContainer;
    private readonly ActivitySource _activitySource;

    public AzureCosmosDb(CosmosClient client, IConfiguration configuration, InstrumentationConfig instrumentation)
    {
        _photoContainer = client.GetDatabase(configuration["CosmosDb:DatabaseId"])
            .GetContainer(configuration["CosmosDb:ContainerId"]);
        _folderContainer = client.GetDatabase(configuration["CosmosDb:DatabaseId"])
            .GetContainer(configuration["CosmosDb:FoldersContainerId"]);
        _albumContainer = client.GetDatabase(configuration["CosmosDb:DatabaseId"])
            .GetContainer(configuration["CosmosDb:AlbumsContainerId"]);
        _sessionDataContainer = client.GetDatabase(configuration["CosmosDb:DatabaseId"])
            .GetContainer(configuration["CosmosDb:SessionDataContainerId"]);
        _userContainer = client.GetDatabase(configuration["CosmosDb:DatabaseId"])
            .GetContainer(configuration["CosmosDb:UsersContainerId"]);
        _activitySource = instrumentation.ActivitySource;
    }
    public async Task SavePhoto(Photo photo)
    {
        using var activity = _activitySource.StartActivity("store photo data in database");

        var cosmosPhoto = new CosmosPhoto(photo.Id.Id, photo.Id.FolderId, photo.Id.AlbumId, photo.Title, photo.Metadata);

        await _photoContainer.UpsertItemAsync(cosmosPhoto, ToPartitionKey(photo.Id));

        activity?.AddEvent(new ActivityEvent("photo data stored in database"));
    }

    public async Task<IReadOnlyCollection<Photo>> ListPhotos(AlbumId albumId)
    {
        using var activity = _activitySource.StartActivity("retrieving photos from database");

        var query = new QueryDefinition("SELECT * FROM c WHERE c.folderId = @folderId AND c.albumId = @albumId")
            .WithParameter("@folderId", albumId.FolderId)
            .WithParameter("@albumId", albumId.Id);

        var photos = new List<Photo>();
        var iterator = _photoContainer.GetItemQueryIterator<CosmosPhoto>(query, requestOptions: new() { PartitionKey = ToPartitionKey(albumId) });

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();

            photos.AddRange(response.Select(x => x.ToPhotoEntity()));
        }

        activity?.AddEvent(new ActivityEvent("photos retrieved from database"));

        return photos;
    }

    public async Task RemovePhoto(PhotoId photoId)
    {
        using var activity = _activitySource.StartActivity("removing photo from database");

        await _photoContainer.DeleteItemAsync<CosmosPhoto>(photoId.Id.ToString(), ToPartitionKey(photoId));

        activity?.AddEvent(new ActivityEvent("photo removed from database"));
    }

    public async Task<Photo> GetPhoto(PhotoId photoId)
    {
        using var activity = _activitySource.StartActivity("retrieving photo from database");

        var response = await _photoContainer.ReadItemAsync<CosmosPhoto>(photoId.Id.ToString(), ToPartitionKey(photoId));

        activity?.AddEvent(new ActivityEvent("photo retrieved from database"));

        return response.Resource.ToPhotoEntity();
    }

    public async Task StoreFolder(Folder folder)
    {
        using var activity = _activitySource.StartActivity("store folder in database");

        var cosmosFolder = new CosmosFolder(folder.Id, folder.ParentId, folder.Name.Value);

        await _folderContainer.CreateItemAsync(cosmosFolder, new(folder.ParentId.ToString()));

        activity?.AddEvent(new ActivityEvent("folder stored in database"));
    }

    public async Task<IReadOnlyCollection<Folder>> GetFolders(Guid parentId)
    {
        using var activity = _activitySource.StartActivity("retrieving folders from database");

        var query = new QueryDefinition("SELECT * FROM c WHERE c.parentId = @parentId")
            .WithParameter("@parentId", parentId);
        var folders = new List<Folder>();

        var iterator = _folderContainer.GetItemQueryIterator<CosmosFolder>(query, requestOptions: new() { PartitionKey = new PartitionKey(parentId.ToString()) });
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            folders.AddRange(response.Select(x => new Folder(x.Id, x.ParentId, new(x.Name))));
        }

        activity?.AddEvent(new ActivityEvent("folders retrieved from database"));

        return folders;
    }

    public async Task<Folder> GetFolder(Guid parentId, Guid folderId)
    {
        using var activity = _activitySource.StartActivity("retrieving folder from database");

        var response = await _folderContainer.ReadItemAsync<CosmosFolder>(folderId.ToString(), new PartitionKey(parentId.ToString()));

        activity?.AddEvent(new ActivityEvent("folder retrieved from database"));

        return new Folder(response.Resource.Id, response.Resource.ParentId, new(response.Resource.Name));
    }

    public async Task RemoveFolder(Guid parentId, Guid folderId)
    {
        using var activity = _activitySource.StartActivity("removing folder from database");

        await _folderContainer.DeleteItemAsync<CosmosFolder>(folderId.ToString(), new PartitionKey(parentId.ToString()));

        activity?.AddEvent(new ActivityEvent("folder removed from database"));
    }

    public async Task UpsertFolder(Guid parentId, Guid folderId, Name name)
    {
        using var activity = _activitySource.StartActivity("upsert folder in database");

        var folder = new CosmosFolder(folderId, parentId, name.Value);

        await _folderContainer.UpsertItemAsync(folder, new PartitionKey(parentId.ToString()));

        activity?.AddEvent(new ActivityEvent("folder upserted in database"));
    }

    public async Task<IReadOnlyCollection<Album>> GetAlbums(Guid folderId)
    {
        using var activity = _activitySource.StartActivity("retrieving albums from database");

        var query = new QueryDefinition("SELECT * FROM c WHERE c.folderId = @folderId")
            .WithParameter("@folderId", folderId);
        var albums = new List<Album>();

        var iterator = _albumContainer.GetItemQueryIterator<CosmosAlbum>(query, requestOptions: new() { PartitionKey = new PartitionKey(folderId.ToString()) });
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            albums.AddRange(response.Select(x => new Album(x.Id, x.FolderId, new(x.Name))));
        }

        activity?.AddEvent(new ActivityEvent("albums retrieved from database"));

        return albums;
    }

    public async Task StoreAlbum(Album album)
    {
        using var activity = _activitySource.StartActivity("store album in database");

        var cosmosAlbum = new CosmosAlbum(album.Id, album.FolderId, album.Name.Value);

        await _albumContainer.CreateItemAsync(cosmosAlbum, new(album.FolderId.ToString()));

        activity?.AddEvent(new ActivityEvent("album stored in database"));
    }

    public async Task<Album> GetAlbum(AlbumId albumId)
    {
        using var activity = _activitySource.StartActivity("retrieving album from database");

        var response = await _albumContainer.ReadItemAsync<CosmosAlbum>(albumId.Id.ToString(), new PartitionKey(albumId.FolderId.ToString()));

        activity?.AddEvent(new ActivityEvent("album retrieved from database"));

        return new Album(response.Resource.Id, response.Resource.FolderId, new(response.Resource.Name));
    }

    public async Task StoreSessionData(Guid userId, SessionData sessionData)
    {
        using var activity = _activitySource.StartActivity("store session data in database");

        var cosmosSessionData = new CosmosSessionData(userId, [.. sessionData.FolderStack.Select(x => new CosmosFolder(x.Id, x.ParentId, x.Name)).Reverse()]);

        await _sessionDataContainer.UpsertItemAsync(cosmosSessionData, new PartitionKey(userId.ToString()));

        activity?.AddEvent(new ActivityEvent("session data stored in database"));
    }

    public async Task<SessionData?> GetSessionData(Guid userId)
    {
        using var activity = _activitySource.StartActivity("retrieving session data from database");

        try
        {
            var response = await _sessionDataContainer.ReadItemAsync<CosmosSessionData>(userId.ToString(), new PartitionKey(userId.ToString()));

            activity?.AddEvent(new ActivityEvent("session data retrieved from database"));

            return new SessionData(new Stack<Components.Models.FolderModel>(response.Resource.FolderStack.Select(x => new Components.Models.FolderModel { Id = x.Id, ParentId = x.ParentId, Name = x.Name })));
        }
        catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            activity?.AddEvent(new ActivityEvent("session data not found"));

            return default;
        }
    }

    public async Task<FotoUser?> FindUser(FotoUserId userId)
    {
        using var activity = _activitySource.StartActivity("retrieving user from database");

        try
        {
            var response = await _userContainer.ReadItemAsync<CosmosUser>(userId.Value, new PartitionKey(userId.Value));

            activity?.AddEvent(new ActivityEvent("user retrieved from database"));

            return new(new FotoUserId(response.Resource.Id), Name.Create(response.Resource.GivenName), response.Resource.RootFolderId);
        }
        catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            activity?.AddEvent(new ActivityEvent("user not found"));

            return default;
        }
    }

    public async Task StoreUser(FotoUser user)
    {
        using var activity = _activitySource.StartActivity("add user to database");

        var cosmosUser = new CosmosUser(user.Id.Value, user.GivenName.Value, user.RootFolderId);

        await _userContainer.UpsertItemAsync(cosmosUser, new PartitionKey(user.Id.Value));

        activity?.AddEvent(new ActivityEvent("user added to database"));
    }

    private static PartitionKey ToPartitionKey(PhotoId photoId) => new PartitionKeyBuilder()
            .Add(photoId.FolderId.ToString())
            .Add(photoId.AlbumId.ToString())
            .Build();

    private static PartitionKey ToPartitionKey(AlbumId albumId) => new PartitionKeyBuilder()
            .Add(albumId.FolderId.ToString())
            .Add(albumId.Id.ToString())
            .Build();
    internal async Task<int> GetAlbumPhotoCount(Guid folderId, Guid albumId)
    {
        using var activity = _activitySource.StartActivity("retrieving album photo count from database");

        var query = new QueryDefinition("SELECT VALUE COUNT(1) FROM c WHERE c.folderId = @folderId AND c.albumId = @albumId")
            .WithParameter("@folderId", folderId)
            .WithParameter("@albumId", albumId);

        var iterator = _photoContainer.GetItemQueryIterator<int>(query, requestOptions: new() { PartitionKey = new PartitionKey(folderId.ToString()) });
        var count = 0;
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            count += response.Sum();
        }

        activity?.AddEvent(new ActivityEvent("album photo count retrieved from database"));

        return count;
    }

    private sealed record CosmosPhoto(Guid Id, Guid FolderId, Guid AlbumId, string Title, ExifMetadata? Metadata)
    {
        public Photo ToPhotoEntity() => new(
                new PhotoId(FolderId, AlbumId, Id),
                Title,
                Metadata);
    }

    private sealed record CosmosFolder(Guid Id, Guid ParentId, string Name);

    private sealed record CosmosAlbum(Guid Id, Guid FolderId, string Name);

    private sealed record CosmosSessionData(Guid Id, CosmosFolder[] FolderStack);

    private sealed record CosmosUser(string Id, string GivenName, Guid RootFolderId);
}
