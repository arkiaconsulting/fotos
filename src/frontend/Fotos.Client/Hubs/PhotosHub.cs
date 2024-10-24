﻿using Microsoft.AspNetCore.SignalR;

namespace Fotos.Client.Hubs;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Required by Function runtime")]
public sealed class PhotosHub : Hub
{
    public async Task SendThumbnailReady(PhotoId id)
    {
        await Clients.All.SendAsync("ThumbnailReady", id);
    }
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Required by Function runtime")]
public readonly record struct PhotoId(Guid FolderId, Guid AlbumId, Guid Id);