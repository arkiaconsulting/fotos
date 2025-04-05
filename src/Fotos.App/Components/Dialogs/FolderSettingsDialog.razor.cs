using Fotos.App.Components.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fotos.App.Components.Dialogs;
public partial class FolderSettingsDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter]
    public FolderModel Folder { get; set; } = default!;

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
        MudDialog.Close(DialogResult.Ok(Folder));

        if (_previousFolderName != Folder.Name)
        {
            await UpdateFolder(Folder.ParentId, Folder.Id, Folder.Name);
        }
        _previousFolderName = string.Empty;
    }

    private void CancelChanges()
    {
        Folder.Name = _previousFolderName;
        _previousFolderName = string.Empty;

        MudDialog.Close(DialogResult.Cancel());
    }
}