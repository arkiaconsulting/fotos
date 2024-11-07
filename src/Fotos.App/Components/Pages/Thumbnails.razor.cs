using Fotos.App.Components.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fotos.App.Components.Pages;
public partial class Thumbnails
{
    [Parameter]
    public Guid FolderId { get; set; }

    [Parameter]
    public Guid AlbumId { get; set; }

    [Parameter]
    public EventCallback OnLoaded { get; set; }

    [Parameter]
    public EventCallback OnPhotoRemoved { get; set; }

    public int Count => _thumbnails.Count;

    private bool _isPhotoDisplayed;

    private IEnumerable<PhotoModel> FilteredPhotos
    {
        get
        {
            var filtered = _thumbnails.Where(p => p.Title.Contains(_filter ?? string.Empty, StringComparison.InvariantCultureIgnoreCase));

            return _sortDate switch
            {
                SortDirection.Ascending => filtered.OrderBy(p => p.Metadata.DateTaken),
                SortDirection.Descending => filtered.OrderByDescending(p => p.Metadata.DateTaken),
                _ => filtered
            };
        }
    }

    private List<PhotoModel> _thumbnails = [];
    private PhotoModel _photo = PhotoModel.Default();
    private bool _showDetails;
    private string? _filter;
    private SortDirection _sortDate = SortDirection.Ascending;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _thumbnails = (await ListPhotos(new(FolderId, AlbumId))).Select(p => new PhotoModel(p.FolderId, p.AlbumId, p.Id, p.Title, p.Metadata)).ToList();
            await SetThumbnailUris();

            await OnLoaded.InvokeAsync(null);

            StateHasChanged();
        }
    }

    internal void AddPhoto(PhotoModel photo)
    {
        _thumbnails.Add(photo);

        StateHasChanged();
    }

    private async Task RemoveAPhoto(PhotoModel photo)
    {
        await RemovePhoto(new(photo.FolderId, photo.AlbumId, photo.Id));
        _thumbnails.Remove(photo);
        CloseDetails();

        StateHasChanged();

        await OnPhotoRemoved.InvokeAsync(null);
    }

    private async Task ViewPhoto(PhotoModel photo)
    {
        var originalUri = await GetOriginalUri(new(photo.FolderId, photo.AlbumId, photo.Id));

        photo.OriginalUri = originalUri;
        _photo = photo;
        _isPhotoDisplayed = true;
        _showDetails = false;

        StateHasChanged();
    }

    private void DismissPhoto()
    {
        _isPhotoDisplayed = false;
    }

    private async Task SetThumbnailUris()
    {
        foreach (var photo in _thumbnails)
        {
            photo.ThumbnailUri = await GetThumbnailUri(new(photo.FolderId, photo.AlbumId, photo.Id));
        }
    }

    private void ShowDetails(PhotoModel photo)
    {
        _photo = photo;
        _showDetails = true;
    }

    private async Task PhotoRenamed(string newValue)
    {
        if (_photo is null || newValue == _photo!.Title)
        {
            return;
        }

        _photo.Title = newValue;

        await UpdatePhoto(new(_photo.FolderId, _photo.AlbumId, _photo.Id), newValue);
    }

    private void CloseDetails()
    {
        _showDetails = false;
    }

    public async Task OnNewThumbnail(Guid folderId, Guid albumId, Guid id)
    {
        var photo = _thumbnails.FirstOrDefault(p => p.FolderId == folderId && p.AlbumId == albumId && p.Id == id);
        if (photo is null)
        {
            return;
        }

        photo.ThumbnailUri = await GetThumbnailUri(new(folderId, albumId, id));
        StateHasChanged();
    }

    public async Task OnMetadataReady(Guid folderId, Guid albumId, Guid id)
    {
        var photo = _thumbnails.FirstOrDefault(p => p.FolderId == folderId && p.AlbumId == albumId && p.Id == id);
        if (photo is null)
        {
            return;
        }

        var actualPhoto = await GetPhoto(new(folderId, albumId, id));

        photo.Metadata = actualPhoto.Metadata;
        StateHasChanged();
    }

    private void FilterChanged(string newValue)
    {
        if (string.IsNullOrWhiteSpace(newValue))
        {
            _filter = null;
            return;
        }
    }
}