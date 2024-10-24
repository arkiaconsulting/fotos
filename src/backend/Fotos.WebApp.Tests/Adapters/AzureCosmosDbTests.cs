﻿using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.Client.Api.Photos;
using Fotos.WebApp.Tests.Assets;
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
}
