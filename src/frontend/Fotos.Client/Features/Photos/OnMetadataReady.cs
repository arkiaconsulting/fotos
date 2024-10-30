using Fotos.Client.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.WebJobs;

namespace Fotos.Client.Features.Photos;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Required by Function runtime")]
public sealed class OnMetadataReady
{
    private readonly IHubContext<PhotosHub> _hub;

    public OnMetadataReady(IHubContext<PhotosHub> hub) => _hub = hub;

    [FunctionName("OnMetadataReady")]
    public async Task Handle(
        [ServiceBusTrigger("%ServiceBus:MainTopic%", "%ServiceBus:MetadataReadySubscription%", AutoCompleteMessages = true, Connection = "ServiceBus")] PhotoId photoId)
    {
        await _hub.Clients.All.SendAsync("MetadataReady", photoId);
    }
}
