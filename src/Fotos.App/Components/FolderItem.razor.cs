using Fotos.App.Components.Dialogs;
using Fotos.App.Components.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fotos.App.Components;
public partial class FolderItem
{
    [Parameter]
    public FolderModel Folder { get; set; } = default!;

    [Parameter]
    public EventCallback<FolderModel> OnFolderChanged { get; set; }

    [Parameter]
    public EventCallback<FolderModel> OnFolderRemoved { get; set; }

    private bool _isOverlayVisible;

    private async Task FolderClicked()
    {
        await OnFolderChanged.InvokeAsync(Folder);
    }

    private async Task RemoveThisFolder()
    {
        var childFoldersCount = (await ListFolders(Folder.Id)).Count;

        if (childFoldersCount > 0)
        {
            Snackbar.Add("This folder contains child folders. Please remove them first.", Severity.Error);

            return;
        }

        await OnFolderRemoved.InvokeAsync(Folder);
    }

    private async Task OpenSettings()
    {
        var parameters = new DialogParameters<FolderSettingsDialog> { { x => x.Folder, Folder } };
        var dialog = await DialogService.ShowAsync<FolderSettingsDialog>(default, parameters);
        _ = await dialog.Result;
    }
}