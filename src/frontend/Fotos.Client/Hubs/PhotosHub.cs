using Fotos.Client.Features.Photos;
using Microsoft.AspNetCore.SignalR;

namespace Fotos.Client.Hubs;

public sealed class PhotosHub : Hub
{
    public async Task SendThumbnailReady(PhotoId id)
    {
        await Clients.All.SendAsync("ThumbnailReady", id);
    }
}