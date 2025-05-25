using Fotos.App.Components.Dialogs;
using Fotos.App.Components.Models;
using Fotos.Application;
using Fotos.Application.Albums;
using Fotos.Application.Folders;
using Fotos.Application.User;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;

namespace Fotos.App.Components.Pages.Restricted;
public partial class Home
{
    [CascadingParameter]
    public ProcessError? ProcessError { get; set; }

    [CascadingParameter]
    public HttpContext HttpContext { get; set; } = default!;

    [Inject]
    internal ISender Sender { get; set; } = default!;

    public FolderModel CurrentFolder => FolderModel.From(SessionData.FolderStack.Peek());

    private List<FolderModel> _childFolders = [];
    private List<AlbumModel> _childAlbums = [];

    private string _newFolder = string.Empty;
    private string _newAlbumName = string.Empty;
    private bool _loaded;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            using var activity = DiagnosticConfig.StartUserActivity("Home: Initialize");

            try
            {
                if (SessionData.FolderStack.Count == 0)
                {
                    // We're at root
                    var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                    var userProviderId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var provider = authState.User.Identity?.AuthenticationType;

                    var userResult = await Sender.Send(new FindUserQuery(provider!, userProviderId!), CancellationToken.None);
                    var fotoUser = userResult.Value;

                    var result = await Sender.Send(new GetFolderQuery(Guid.Empty, fotoUser!.Value.RootFolderId), CancellationToken.None);

                    SessionData.FolderStack.Push(result.Value);
                }

                await RefreshFoldersAndAlbums();

                _loaded = true;
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, "Cannot initialize Home page");
                activity?.AddException(ex);
                ProcessError?.LogError(ex);
            }
        }
    }

    private async Task RefreshFolders()
    {
        try
        {
            var result = await Sender.Send(new ListChildFoldersQuery(CurrentFolder.Id), CancellationToken.None);

            _childFolders = [.. result.Value.Select(dto => new FolderModel { Id = dto.Id, ParentId = dto.ParentId, Name = dto.Name.Value })];
        }
        catch (Exception ex)
        {
            ProcessError?.LogError(ex);
        }
    }

    private async Task RefreshAlbums()
    {
        try
        {
            var result = await Sender.Send(new ListFolderAlbumsQuery(CurrentFolder.Id), CancellationToken.None);

            _childAlbums = [.. result.Value.Select(dto => new AlbumModel { Id = dto.Album.Id, FolderId = dto.Album.FolderId, Name = dto.Album.Name.Value, PhotoCount = dto.PhotoCount })];
        }
        catch (Exception ex)
        {
            ProcessError?.LogError(ex);
        }
    }

    private async Task RefreshFoldersAndAlbums()
    {
        try
        {
            await Task.WhenAll(RefreshFolders(), RefreshAlbums());
        }
        catch (Exception ex)
        {
            ProcessError?.LogError(ex);
        }
    }

    private async Task CreateNewFolder()
    {
        using var activity = DiagnosticConfig.StartUserActivity("Home: Create new folder");

        try
        {
            var command = new CreateFolderCommand(CurrentFolder.Id, _newFolder);

            var result = await Sender.Send(command);

            await RefreshFolders();
            _newFolder = string.Empty;
        }
        catch (Exception ex)
        {
            ProcessError?.LogError(ex);
        }
    }

    private async Task GoToParentFolder()
    {
        using var activity = DiagnosticConfig.StartUserActivity("Home: Go to parent folder");

        _ = SessionData.FolderStack.Pop();

        try
        {
            await RefreshFoldersAndAlbums();
        }
        catch (Exception ex)
        {
            ProcessError?.LogError(ex);
        }
    }

    private async Task RemoveThisFolder(FolderModel folder)
    {
        using var activity = DiagnosticConfig.StartUserActivity("Home: Remove folder");

        try
        {
            var command = new RemoveFolderCommand(folder.ParentId, folder.Id);

            var result = await Sender.Send(command);

            _childFolders.Remove(folder);
        }
        catch (Exception ex)
        {
            ProcessError?.LogError(ex);
        }
    }

    private async Task CreateNewAlbum()
    {
        using var activity = DiagnosticConfig.StartUserActivity("Home: Create new album");

        try
        {
            var command = new CreateAlbumCommand(CurrentFolder.Id, _newAlbumName);
            var result = await Sender.Send(command);

            await RefreshAlbums();
            _newAlbumName = string.Empty;
        }
        catch (Exception ex)
        {
            ProcessError?.LogError(ex);
        }
    }

    private async Task GoToFolder(FolderModel folder)
    {
        using var activity = DiagnosticConfig.StartUserActivity("Home: Go to folder");

        SessionData.FolderStack.Push(folder.Map());

        try
        {
            await RefreshFoldersAndAlbums();
        }
        catch (Exception ex)
        {
            ProcessError?.LogError(ex);
        }
    }

    private void GoToAlbum(Guid albumId)
    {
        using var activity = DiagnosticConfig.StartUserActivity("Home: Go to album");

        NavigationManager.NavigateTo($"album/{CurrentFolder.ParentId}/{CurrentFolder.Id}/{albumId}");
    }

    private async Task OpenCurrentFolderSettings()
    {
        using var activity = DiagnosticConfig.StartUserActivity("Home: Open folder settings");

        var parameters = new DialogParameters<FolderSettingsDialog> { { x => x.Folder, CurrentFolder } };
        var dialog = await DialogService.ShowAsync<FolderSettingsDialog>(default, parameters);
        _ = await dialog.Result;
    }
}