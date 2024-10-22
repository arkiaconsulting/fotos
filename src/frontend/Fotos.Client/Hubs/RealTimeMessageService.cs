using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Fotos.Client.Hubs;

internal class RealTimeMessageService : IAsyncDisposable
{
    public event EventHandler<PhotoId>? OnThumbnailReady;

    private readonly HubConnection _hubConnection;

    public RealTimeMessageService(NavigationManager navigationManager)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri("/photoshub"))
        .Build();

        _hubConnection.On<PhotoId>("ThumbnailReady", id => OnThumbnailReady?.Invoke(this, id));
    }

    public virtual async Task StartAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.StartAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _hubConnection.DisposeAsync();
    }
}
