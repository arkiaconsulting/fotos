using Fotos.App.Components.Models;
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
        await OnAlbumChanged.InvokeAsync(Album);
    }
}