using Fotos.App.Domain;
using Microsoft.AspNetCore.SignalR;

namespace Fotos.App.Adapters.RealTimeMessaging;

public sealed class PhotosHub : Hub
{
    public async Task SendThumbnailReady(PhotoId id)
    {
        await Clients.All.SendAsync("ThumbnailReady", id);
    }
}