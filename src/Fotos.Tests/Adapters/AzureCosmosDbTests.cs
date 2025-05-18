using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.App.Application.User;
using Fotos.App.Components.Models;
using Fotos.App.Domain;
using Fotos.Tests.Assets;
using Microsoft.Azure.Cosmos;
using System.Net;
using System.Text.Json;

namespace Fotos.Tests.Adapters;

[Trait("Category", "Integration")]
public sealed class AzureCosmosDbTests : IClassFixture<FotoIntegrationContext>
{
    private readonly FotoIntegrationContext _context;
    public AzureCosmosDbTests(FotoIntegrationContext context) => _context = context;

    [Theory(DisplayName = "When adding a new photo should store its data"), AutoData]
    internal async Task Test01(PhotoId photoId, string title, ExifMetadata metadata)
    {
        var photo = new Photo(photoId, title, metadata);

        await _context.StorePhotoData(photo);

        var partitionKey = new PartitionKeyBuilder().Add(photo.Id.FolderId.ToString()).Add(photo.Id.AlbumId.ToString()).Build();
        var response = await _context.PhotosData.ReadItemStreamAsync(photo.Id.Id.ToString(), partitionKey);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var item = await JsonSerializer.DeserializeAsync<JsonElement>(response.Content, options: new(JsonSerializerDefaults.Web));
        item.GetProperty("folderId").GetGuid().Should().Be(photo.Id.FolderId);
        item.GetProperty("albumId").GetGuid().Should().Be(photo.Id.AlbumId);
        item.GetProperty("title").GetString().Should().Be(photo.Title);
        item.GetProperty("metadata").GetProperty("dateTaken").GetDateTime().Should().Be(photo.Metadata?.DateTaken);
    }

    [Theory(DisplayName = "When listing photo data of an album should return data"), AutoData]
    internal async Task Test02(Photo photo0, Photo photo1)
    {
        await _context.StorePhotoData(photo0);
        await _context.StorePhotoData(photo1);

        var actualPhotos = await _context.ListPhotos(new(photo0.Id.FolderId, photo0.Id.AlbumId));

        actualPhotos.Should().BeEquivalentTo([photo0]);
    }

    [Theory(DisplayName = "When removing an existing photo should remove it"), AutoData]
    internal async Task Test03(Photo photo)
    {
        await _context.StorePhotoData(photo);

        await _context.RemovePhotoData(photo.Id);

        var partitionKey = new PartitionKeyBuilder().Add(photo.Id.FolderId.ToString()).Add(photo.Id.AlbumId.ToString()).Build();
        var response = await _context.PhotosData.ReadItemStreamAsync(photo.Id.Id.ToString(), partitionKey);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory(DisplayName = "When getting an existing photo should return it"), AutoData]
    internal async Task Test04(Photo photo)
    {
        await _context.StorePhotoData(photo);

        var actualPhoto = await _context.GetPhoto(photo.Id);

        actualPhoto.Should().BeEquivalentTo(photo);
    }

    [Theory(DisplayName = "When storing session data should effectively store it"), AutoData]
    internal async Task Test05(SessionData sessionData, FolderModel folder1, FolderModel folder2, Guid userId)
    {
        sessionData.FolderStack.Push(folder1);
        sessionData.FolderStack.Push(folder2);

        await _context.StoreSessionData(userId, sessionData);

        var queryDefinition = new QueryDefinition("select * from c where c.id = @userId").WithParameter("@userId", userId.ToString());
        var iterator = _context.SessionData.GetItemQueryStreamIterator(queryDefinition, requestOptions: new() { PartitionKey = new(userId.ToString()) });
        var result = default(JsonElement);
        while (iterator.HasMoreResults)
        {
            using var response = await iterator.ReadNextAsync();
            result = (await BinaryData.FromStreamAsync(response.Content)).ToObjectFromJson<JsonElement>().GetProperty("Documents");
        }

        result.ValueKind.Should().Be(JsonValueKind.Array);
        result.GetArrayLength().Should().Be(1);
        result.EnumerateArray().Single().GetProperty("folderStack").EnumerateArray().Select(x => x.GetProperty("id").GetGuid())
            .Should().BeEquivalentTo([folder1.Id, folder2.Id], config => config.WithStrictOrdering());
        result.EnumerateArray().Single().GetProperty("folderStack").EnumerateArray().Select(x => x.GetProperty("parentId").GetGuid())
            .Should().BeEquivalentTo([folder1.ParentId, folder2.ParentId], config => config.WithStrictOrdering());
    }

    [Theory(DisplayName = "When fetching stored session data should pass"), AutoData]
    internal async Task Test06(SessionData sessionData, FolderModel folder1, FolderModel folder2, Guid userId)
    {
        sessionData.FolderStack.Push(folder1);
        sessionData.FolderStack.Push(folder2);
        await _context.StoreSessionData(userId, sessionData);

        var actualSessionData = await _context.GetSessionData(userId);

        actualSessionData!.FolderStack.Should().BeEquivalentTo(sessionData.FolderStack, config => config.WithStrictOrdering());
    }

    [Theory(DisplayName = "Add user to store should pass"), AutoData]
    internal async Task Test07(FotoUser user)
    {
        await _context.AddUserToStore(user);

        var response = await _context.UsersData.ReadItemStreamAsync(user.Id.Value, new(user.Id.Value));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var item = await JsonSerializer.DeserializeAsync<JsonElement>(response.Content, options: new(JsonSerializerDefaults.Web));
        item.GetProperty("givenName").GetString().Should().Be(user.GivenName.Value);
    }

    [Theory(DisplayName = "When getting an album photo count should return right count"), AutoData]
    internal async Task Test08(Photo[] photos, Photo anotherPhoto)
    {
        await Task.WhenAll(photos.Select(photo => _context.StorePhotoData(photo with { Id = photo.Id with { FolderId = photos[0].Id.FolderId, AlbumId = photos[0].Id.AlbumId } })));
        await _context.StorePhotoData(anotherPhoto);

        var actualCount = await _context.GetAlbumPhotoCount(photos[0].Id.FolderId, photos[0].Id.AlbumId);

        actualCount.Should().Be(photos.Length);
    }
}
