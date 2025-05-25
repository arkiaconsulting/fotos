using Fotos.App.Components.Dialogs;
using Fotos.App.Components.Models;
using Fotos.Application;
using Fotos.Application.Folders;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fotos.App.Components;
public partial class FolderItem
{
    [CascadingParameter]
    public ProcessError? ProcessError { get; set; }

    [Parameter]
    public FolderModel Folder { get; set; } = default!;

    [Parameter]
    public EventCallback<FolderModel> OnFolderChanged { get; set; }

    [Parameter]
    public EventCallback<FolderModel> OnFolderRemoved { get; set; }

    [Inject]
    internal ISender Sender { get; set; } = default!;

    private bool _isOverlayVisible;

    private async Task FolderClicked()
    {
        using var activity = DiagnosticConfig.StartUserActivity("Folder clicked");

        await OnFolderChanged.InvokeAsync(Folder);
    }

    private async Task RemoveThisFolder()
    {
        using var activity = DiagnosticConfig.StartUserActivity("Remove folder");

        try
        {
            var result = await Sender.Send(new ListChildFoldersQuery(Folder.Id));
            var childFoldersCount = result.Value.Count;

            if (childFoldersCount > 0)
            {
                Snackbar.Add("This folder contains child folders. Please remove them first.", Severity.Error);

                return;
            }
        }
        catch (Exception ex)
        {
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, "Unable to list child folders");
            activity?.AddException(ex);
            ProcessError?.LogError(ex);
        }

        await OnFolderRemoved.InvokeAsync(Folder);
    }

    private async Task OpenSettings()
    {
        using var activity = DiagnosticConfig.StartUserActivity("Open folder settings");

        var parameters = new DialogParameters<FolderSettingsDialog> { { x => x.Folder, Folder } };
        var dialog = await DialogService.ShowAsync<FolderSettingsDialog>(default, parameters);
        _ = await dialog.Result;
    }
}