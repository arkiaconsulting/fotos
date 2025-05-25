using Fotos.App.Components.Models;
using Fotos.Application;
using Fotos.Application.Folders;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fotos.App.Components.Dialogs;
public partial class FolderSettingsDialog
{
    [CascadingParameter]
    public ProcessError? ProcessError { get; set; }

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter]
    public FolderModel Folder { get; set; } = default!;

    [Inject]
    internal ISender Sender { get; set; } = default!;

    private string _previousFolderName = string.Empty;
    private readonly DialogOptions _settingsOptions = new()
    {
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };

    protected override void OnParametersSet()
    {
        _previousFolderName = Folder.Name;

        base.OnParametersSet();
    }

    private async Task SaveChanges()
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity("Save folder settings", System.Diagnostics.ActivityKind.Client);

        MudDialog.Close(DialogResult.Ok(Folder));

        try
        {
            if (_previousFolderName != Folder.Name)
            {
                var command = new RenameFolderCommand(Folder.ParentId, Folder.Id, Folder.Name);

                var result = await Sender.Send(command);
            }
            _previousFolderName = string.Empty;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, "Unable to save folder settings");
            activity?.AddException(ex);
            ProcessError?.LogError(ex);
        }
    }

    private void CancelChanges()
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity("Discard folder settings", System.Diagnostics.ActivityKind.Client);

        Folder.Name = _previousFolderName;
        _previousFolderName = string.Empty;

        MudDialog.Close(DialogResult.Cancel());
    }
}