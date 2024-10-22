using Microsoft.AspNetCore.SignalR;

namespace Fotos.Client.Hubs;

internal sealed class PhotosHub : Hub
{
    public async Task SendThumbnailReady(PhotoId id)
    {
        await Clients.All.SendAsync("ThumbnailReady", id);
    }
}

internal readonly record struct PhotoId(Guid FolderId, Guid AlbumId, Guid Id);