using AutoFixture.Xunit2;
using FluentAssertions;
using Fotos.WebApp.Features.Photos;
using Fotos.WebApp.Tests.Assets;
using System.Net.Mime;
using System.Text.Json;

namespace Fotos.WebApp.Tests.Adapters;

[Trait("Category", "Integration")]
public sealed class AzureServiceBusTests : IClassFixture<FotoIntegrationContext>
{
    private readonly FotoIntegrationContext _context;
    public AzureServiceBusTests(FotoIntegrationContext context) => _context = context;

    [Theory(DisplayName = "When adding a new photo should inform that a new photo is available for being processed"), AutoData]
    internal async Task Test01(PhotoId photoId)
    {
        await _context.OnNewPhotoUploaded(photoId);

        var receiver = _context.ServiceBusClient.CreateReceiver(_context.MainTopicName, "photo-uploaded", options: new() { ReceiveMode = Azure.Messaging.ServiceBus.ServiceBusReceiveMode.ReceiveAndDelete });
        var messages = await receiver.ReceiveMessagesAsync(1, maxWaitTime: TimeSpan.FromSeconds(2));
        var message = messages.Should().ContainSingle().Subject;

        message.Subject.Should().Be("PhotoUploaded");
        message.ContentType.Should().Be(MediaTypeNames.Application.Json);
        var payload = message.Body.ToObjectFromJson<PhotoId>(options: new(JsonSerializerDefaults.Web));
        payload.Should().BeEquivalentTo(photoId);
    }

    [Theory(DisplayName = "When removing a photo should inform by publishing"), AutoData]
    internal async Task Test02(PhotoId photoId)
    {
        await _context.OnPhotoRemoved(photoId);

        var receiver = _context.ServiceBusClient.CreateReceiver(_context.MainTopicName, "photo-removed", options: new() { ReceiveMode = Azure.Messaging.ServiceBus.ServiceBusReceiveMode.ReceiveAndDelete });
        var messages = await receiver.ReceiveMessagesAsync(1, maxWaitTime: TimeSpan.FromSeconds(2));
        var message = messages.Should().ContainSingle().Subject;

        message.Subject.Should().Be("PhotoRemoved");
        message.ContentType.Should().Be(MediaTypeNames.Application.Json);
        var payload = message.Body.ToObjectFromJson<PhotoId>(options: new(JsonSerializerDefaults.Web));
        payload.Should().BeEquivalentTo(photoId);
    }

    [Theory(DisplayName = "When a thumbnail has been generated, should inform by publishing"), AutoData]
    internal async Task Test03(PhotoId photoId)
    {
        await _context.OnThumbnailReady(photoId);

        var receiver = _context.ServiceBusClient.CreateReceiver(_context.MainTopicName, "thumbnail-ready", options: new() { ReceiveMode = Azure.Messaging.ServiceBus.ServiceBusReceiveMode.ReceiveAndDelete });
        var messages = await receiver.ReceiveMessagesAsync(1, maxWaitTime: TimeSpan.FromSeconds(2));
        var message = messages.Should().ContainSingle().Subject;
        message.Subject.Should().Be("ThumbnailReady");
        message.ContentType.Should().Be(MediaTypeNames.Application.Json);
        var payload = message.Body.ToObjectFromJson<PhotoId>(options: new(JsonSerializerDefaults.Web));
        payload.Should().BeEquivalentTo(photoId);
    }
}
