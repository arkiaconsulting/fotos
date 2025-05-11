using Fotos.App.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Fotos.App.Hubs;

internal class RealTimeMessageService : IAsyncDisposable
{
    public event EventHandler<PhotoId>? OnThumbnailReady;
    public event EventHandler<PhotoId>? OnMetadataReady;

    private readonly HubConnection _hubConnection;

    public RealTimeMessageService(NavigationManager navigationManager)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri("/photoshub"))
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<PhotoId>("ThumbnailReady", id => OnThumbnailReady?.Invoke(this, id));
        _hubConnection.On<PhotoId>("MetadataReady", id => OnMetadataReady?.Invoke(this, id));
    }

    public virtual async Task StartAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.StartAsync();
        }
    }

    public virtual async ValueTask DisposeAsync()
    {
        await _hubConnection.DisposeAsync();
    }
}
