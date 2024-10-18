using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.WebApp.Tests.Assets;
using Fotos.WebApp.Types;
using Microsoft.Azure.Cosmos;
using System.Net;
using System.Text.Json;

namespace Fotos.WebApp.Tests.Adapters;

[Trait("Category", "Integration")]
public sealed class AzureCosmosDbTests : IClassFixture<FotoIntegrationContext>
{
    private readonly FotoIntegrationContext _context;
    public AzureCosmosDbTests(FotoIntegrationContext context) => _context = context;

    [Theory(DisplayName = "When adding a new photo should store its data"), AutoData]
    internal async Task Test01(PhotoId photoId, string title, ExifMetadata metadata)
    {
        var photo = new PhotoEntity(photoId, title, metadata);

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
}
