using Fotos.App.Adapters;
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
            StateHasChanged();
        }
    }

    private async Task RefreshFolders()
    {
        _childFolders = [.. (await ListChildFolders.Process(CurrentFolder.Id)).Select(dto => new FolderModel { Id = dto.Id, ParentId = dto.ParentId, Name = dto.Name.Value })];
    }

    private async Task RefreshAlbums()
    {
        _childAlbums = [.. (await ListAlbums(CurrentFolder.Id)).Select(dto => new AlbumModel { Id = dto.Id, FolderId = dto.FolderId, Name = dto.Name, PhotoCount = dto.PhotoCount })];
    }

    private async Task RefreshFoldersAndAlbums()
    {
        await Task.WhenAll(RefreshFolders(), RefreshAlbums());
    }

    private async Task CreateNewFolder()
    {
        await CreateFolder.Process(CurrentFolder.Id, _newFolder);

        await RefreshFolders();
        _newFolder = string.Empty;
    }

    private async Task GotToParentFolder()
    {
        _ = SessionData.FolderStack.Pop();

        await RefreshFoldersAndAlbums();
    }

    private async Task RemoveThisFolder(FolderModel folder)
    {
        await RemoveFolder.Process(folder.ParentId, folder.Id);

        _childFolders.Remove(folder);
    }

    private async Task CreateNewAlbum()
    {
        await CreateAlbum(CurrentFolder.Id, _newAlbumName);
        await RefreshAlbums();
        _newAlbumName = string.Empty;
    }

    private async Task GoToFolder(FolderModel folder)
    {
        SessionData.FolderStack.Push(folder);

        await RefreshFoldersAndAlbums();
    }

    private void GoToAlbum(Guid albumId)
    {
        NavigationManager.NavigateTo($"album/{CurrentFolder.ParentId}/{CurrentFolder.Id}/{albumId}");
    }

    private async Task OpenCurrentFolderSettings()
    {
        var parameters = new DialogParameters<FolderSettingsDialog> { { x => x.Folder, CurrentFolder } };
        var dialog = await DialogService.ShowAsync<FolderSettingsDialog>(default, parameters);
        _ = await dialog.Result;
    }
}