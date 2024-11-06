using Fotos.App.Features.Photos;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Fotos.Tests.Backend.Assets.InMemory.Messaging;

[SuppressMessage("Design", "CA1812", Justification = "Instantiated by DI")]
internal sealed class InMemoryMessagePublisher
{
    private readonly List<PhotoId> _uploadedMessages;
    private readonly List<PhotoId> _removedMessages;

    public InMemoryMessagePublisher(
        [FromKeyedServices("messages-uploaded")] List<PhotoId> uploadedMessages,
        [FromKeyedServices("messages-removed")] List<PhotoId> removedMessages)
    {
        _uploadedMessages = uploadedMessages;
        _removedMessages = removedMessages;
    }

    public Task PublishUploaded(PhotoId photoId)
    {
        _uploadedMessages.Add(photoId);

        return Task.CompletedTask;
    }

    public Task PublishRemoved(PhotoId photoId)
    {
        _removedMessages.Add(photoId);

        return Task.CompletedTask;
    }
}
