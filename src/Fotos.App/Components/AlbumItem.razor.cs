using Fotos.App.Components.Models;
using Fotos.Application;
using Microsoft.AspNetCore.Components;

namespace Fotos.App.Components;
public partial class AlbumItem
{
    [Parameter]
    public AlbumModel Album { get; set; } = default!;

    [Parameter]
    public EventCallback<AlbumModel> OnAlbumChanged { get; set; }

    private async Task AlbumClicked()
    {
        using var activity = DiagnosticConfig.StartUserActivity("AlbumItem: Album clicked");

        await OnAlbumChanged.InvokeAsync(Album);
    }
}