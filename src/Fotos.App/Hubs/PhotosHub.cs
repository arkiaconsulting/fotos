using Fotos.App.Domain;
using Microsoft.AspNetCore.SignalR;

namespace Fotos.App.Hubs;

public sealed class PhotosHub : Hub
{
    public async Task SendThumbnailReady(PhotoId id)
    {
        await Clients.All.SendAsync("ThumbnailReady", id);
    }
}