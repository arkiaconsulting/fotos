using Fotos.Client.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.WebJobs;

namespace Fotos.Client.Features.Photos;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Required by Function runtime")]
public sealed class OnThumbnailReady
{
    private readonly IHubContext<PhotosHub> _hub;

    public OnThumbnailReady(IHubContext<PhotosHub> hub) => _hub = hub;

    [FunctionName("OnThumbnailReady")]
    public async Task Handle(
        [ServiceBusTrigger("%ServiceBus:MainTopic%", "%ServiceBus:ThumbnailReadySubscription%", AutoCompleteMessages = true, Connection = "ServiceBus")] PhotoId photoId)
    {
        await _hub.Clients.All.SendAsync("ThumbnailReady", photoId);
    }
}
