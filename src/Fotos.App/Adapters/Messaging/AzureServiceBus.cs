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
        _activitySource = instrumentation.AppActivitySource;
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

        try
        {
            await _mainTopicSender.SendMessageAsync(message);

            activity?.AddEvent(new ActivityEvent("photo uploaded message published"));
        }
        catch (ServiceBusException ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Unable to publish PhotoUploaded");
            activity?.AddException(ex);

            throw;
        }
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

        try
        {
            await _mainTopicSender.SendMessageAsync(message);

            activity?.AddEvent(new ActivityEvent("photo removed message published"));
        }
        catch (ServiceBusException ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Unable to publish PhotoRemoved");
            activity?.AddException(ex);

            throw;
        }
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

        try
        {
            await _mainTopicSender.SendMessageAsync(message);

            activity?.AddEvent(new ActivityEvent("thumbnail ready message published"));
        }
        catch (ServiceBusException ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Unable to publish ThumbnailReady");
            activity?.AddException(ex);

            throw;
        }
    }
}
