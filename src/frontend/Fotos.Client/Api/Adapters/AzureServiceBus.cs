﻿using Azure.Messaging.ServiceBus;
using Fotos.Client.Api.Photos;
using System.Net.Mime;
using System.Text.Json;

namespace Fotos.Client.Api.Adapters;

internal sealed class AzureServiceBus
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ServiceBusSender _mainTopicSender;

    public AzureServiceBus(
        ServiceBusClient serviceBusClient,
        IConfiguration configuration)
    {
        _serviceBusClient = serviceBusClient;
        _mainTopicSender = _serviceBusClient.CreateSender(configuration["ServiceBus:MainTopic"]);
    }

    public async Task OnNewPhotoUploaded(PhotoId photoId)
    {
        var message = new ServiceBusMessage
        {
            Subject = "PhotoUploaded",
            ContentType = MediaTypeNames.Application.Json,
            Body = new BinaryData(JsonSerializer.Serialize(photoId, options: Constants.JsonSerializerOptions)),
        };

        await _mainTopicSender.SendMessageAsync(message);
    }

    public async Task OnPhotoRemoved(PhotoId photoId)
    {
        var message = new ServiceBusMessage
        {
            Subject = "PhotoRemoved",
            ContentType = MediaTypeNames.Application.Json,
            Body = new BinaryData(JsonSerializer.Serialize(photoId, options: Constants.JsonSerializerOptions)),
        };

        await _mainTopicSender.SendMessageAsync(message);
    }

    internal async Task OnThumbnailReady(PhotoId photoId)
    {
        var message = new ServiceBusMessage
        {
            Subject = "ThumbnailReady",
            ContentType = MediaTypeNames.Application.Json,
            Body = new BinaryData(JsonSerializer.Serialize(photoId, options: Constants.JsonSerializerOptions)),
        };

        await _mainTopicSender.SendMessageAsync(message);
    }
}
