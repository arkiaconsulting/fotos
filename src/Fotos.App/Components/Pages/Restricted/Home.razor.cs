using Fotos.App.Application.Albums;
using Fotos.App.Application.Folders;
using Fotos.App.Application.User;
using Fotos.App.Components.Dialogs;
using Fotos.App.Components.Models;
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
    internal FindUserBusiness FindUser { get; set; } = default!;

    [Inject]
    internal GetFolderBusiness GetFolder { get; set; } = default!;

    [Inject]
    internal RemoveFolderBusiness RemoveFolder { get; set; } = default!;

    [Inject]
    internal ListChildFoldersBusiness ListChildFolders { get; set; } = default!;

    [Inject]
    internal CreateFolderBusiness CreateFolder { get; set; } = default!;

    [Inject]
    internal CreateAlbumBusiness CreateAlbum { get; set; } = default!;

    [Inject]
    internal ListFolderAlbumsBusiness ListFolderAlbums { get; set; } = default!;

    public FolderModel CurrentFolder => SessionData.FolderStack.Peek();

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

                    var fotoUser = await FindUser.Process(provider!, userProviderId!);

                    var folder = await GetFolder.Process(Guid.Empty, fotoUser!.Value.RootFolderId);
                    SessionData.FolderStack.Push(new FolderModel { Id = folder.Id, ParentId = folder.ParentId, Name = folder.Name.Value });
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
            _childFolders = [.. (await ListChildFolders.Process(CurrentFolder.Id)).Select(dto => new FolderModel { Id = dto.Id, ParentId = dto.ParentId, Name = dto.Name.Value })];

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
            _childAlbums = [.. (await ListFolderAlbums.Process(CurrentFolder.Id)).Select(dto => new AlbumModel { Id = dto.Album.Id, FolderId = dto.Album.FolderId, Name = dto.Album.Name.Value, PhotoCount = dto.PhotoCount })];
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
            await CreateFolder.Process(CurrentFolder.Id, _newFolder);

            await RefreshFolders();
            _newFolder = string.Empty;
        }
        catch (Exception ex)
        {
            ProcessError?.LogError(ex);
        }
    }

    private async Task GotToParentFolder()
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
            await RemoveFolder.Process(folder.ParentId, folder.Id);

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
            await CreateAlbum.Process(CurrentFolder.Id, _newAlbumName);
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

        SessionData.FolderStack.Push(folder);

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