using Azure.Messaging.ServiceBus;
using Fotos.App.Domain;
using System.Diagnostics;
using System.Net.Mime;
using System.Text.Json;

namespace Fotos.App.Adapters.Messaging;

internal sealed class AzureServiceBus
{
    private readonly ServiceBusSender _mainTopicSender;
    private readonly ActivitySource _activitySource;

    public AzureServiceBus(
        ServiceBusSender mainTopicSender,
        InstrumentationConfig instrumentation)
    {
        _mainTopicSender = mainTopicSender;
        _activitySource = instrumentation.ActivitySource;
    }

    public async Task OnNewPhotoUploaded(PhotoId photoId)
    {
        using var activity = _activitySource.StartActivity("publishing photo uploaded", ActivityKind.Producer);
        activity?.SetTag("photoId", photoId.Id);
        activity?.SetTag("messageName", "PhotoUploaded");

        var message = new ServiceBusMessage
        {
            Subject = "PhotoUploaded",
            ContentType = MediaTypeNames.Application.Json,
            Body = new BinaryData(JsonSerializer.Serialize(photoId, options: Constants.JsonSerializerOptions)),
        };

        await _mainTopicSender.SendMessageAsync(message);

        activity?.AddEvent(new ActivityEvent("photo uploaded message published"));
    }

    public async Task OnPhotoRemoved(PhotoId photoId)
    {
        using var activity = _activitySource.StartActivity("publishing photo removed", ActivityKind.Producer);
        activity?.SetTag("photoId", photoId.Id);
        activity?.SetTag("messageName", "PhotoRemoved");

        var message = new ServiceBusMessage
        {
            Subject = "PhotoRemoved",
            ContentType = MediaTypeNames.Application.Json,
            Body = new BinaryData(JsonSerializer.Serialize(photoId, options: Constants.JsonSerializerOptions)),
        };

        await _mainTopicSender.SendMessageAsync(message);

        activity?.AddEvent(new ActivityEvent("photo removed message published"));
    }

    public async Task OnThumbnailReady(PhotoId photoId)
    {
        using var activity = _activitySource.StartActivity("publishing thumbnail ready", ActivityKind.Producer);
        activity?.SetTag("photoId", photoId.Id);
        activity?.SetTag("messageName", "ThumbnailReady");

        var message = new ServiceBusMessage
        {
            Subject = "ThumbnailReady",
            ContentType = MediaTypeNames.Application.Json,
            Body = new BinaryData(JsonSerializer.Serialize(photoId, options: Constants.JsonSerializerOptions)),
        };

        await _mainTopicSender.SendMessageAsync(message);

        activity?.AddEvent(new ActivityEvent("thumbnail ready message published"));
    }
}
