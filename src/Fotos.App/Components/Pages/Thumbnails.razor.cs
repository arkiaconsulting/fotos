using Fotos.App.Application.Photos;
using Fotos.App.Components.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fotos.App.Components.Pages;
public partial class Thumbnails
{
    [CascadingParameter]
    public ProcessError? ProcessError { get; set; }

    [Parameter]
    public Guid FolderId { get; set; }

    [Parameter]
    public Guid AlbumId { get; set; }

    [Parameter]
    public EventCallback OnLoaded { get; set; }

    [Parameter]
    public EventCallback OnPhotoRemoved { get; set; }

    [Inject]
    internal ListAlbumPhotosBusiness ListAlbumPhotos { get; set; } = default!;

    [Inject]
    internal RemovePhotoBusiness RemovePhoto { get; set; } = default!;

    [Inject]
    internal GetOriginalPhotoUriBusiness GetOriginalPhotoUri { get; set; } = default!;

    [Inject]
    internal GetPhotoThumbnailUriBusiness GetPhotoThumbnailUri { get; set; } = default!;

    [Inject]
    internal GetPhotoBusiness GetPhoto { get; set; } = default!;

    [Inject]
    internal UpdatePhotoBusiness UpdatePhoto { get; set; } = default!;

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
            using var activity = DiagnosticConfig.StartUserActivity("Thumbnails: Initialize");

            try
            {


                _thumbnails = [.. (await ListAlbumPhotos.Process(new(FolderId, AlbumId))).Select(p => new PhotoModel(FolderId, AlbumId, p.Id.Id, p.Title, p.Metadata ?? new()))];
                await SetThumbnailUris();

                await OnLoaded.InvokeAsync(null);

                StateHasChanged();
            }
            catch (Exception ex)
            {
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, "Cannot initialize Thumbnails page");
                activity?.AddException(ex);
                ProcessError?.LogError(ex);
            }
        }
    }

    internal void AddPhoto(PhotoModel photo)
    {
        _thumbnails.Add(photo);

        StateHasChanged();
    }

    private async Task RemoveAPhoto(PhotoModel photo)
    {
        using var activity = DiagnosticConfig.StartUserActivity("Thumbnails: Remove Photo");

        try
        {
            await RemovePhoto.Process(new(photo.FolderId, photo.AlbumId, photo.Id));
            _thumbnails.Remove(photo);
            CloseDetails();

            StateHasChanged();

            await OnPhotoRemoved.InvokeAsync(null);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, "Cannot remove photo");
            activity?.AddException(ex);
            ProcessError?.LogError(ex);
        }
    }

    private async Task ViewPhoto(PhotoModel photo)
    {
        using var activity = DiagnosticConfig.StartUserActivity("Thumbnails: View Photo");

        try
        {
            photo.OriginalUri = await GetOriginalPhotoUri.Process(new(photo.FolderId, photo.AlbumId, photo.Id));
            _photo = photo;
            _isPhotoDisplayed = true;
            _showDetails = false;

            StateHasChanged();
        }
        catch (Exception ex)
        {
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, "Cannot view photo");
            activity?.AddException(ex);
            ProcessError?.LogError(ex);
        }
    }

    private void DismissPhoto()
    {
        using var activity = DiagnosticConfig.StartUserActivity("Thumbnails: Dismiss Photo");

        _isPhotoDisplayed = false;
    }

    private async Task SetThumbnailUris()
    {
        try
        {
            foreach (var photo in _thumbnails)
            {
                photo.ThumbnailUri = await GetPhotoThumbnailUri.Process(new(photo.FolderId, photo.AlbumId, photo.Id));
            }
        }
        catch (Exception ex)
        {
            ProcessError?.LogError(ex);
        }
    }

    private void ShowDetails(PhotoModel photo)
    {
        using var activity = DiagnosticConfig.StartUserActivity("Thumbnails: Show Details");

        _photo = photo;
        _showDetails = true;
    }

    private async Task PhotoRenamed(string newValue)
    {
        using var activity = DiagnosticConfig.StartUserActivity("Thumbnails: Rename Photo");

        if (_photo is null || newValue == _photo!.Title)
        {
            return;
        }

        _photo.Title = newValue;

        try
        {
            await UpdatePhoto.Process(new(_photo.FolderId, _photo.AlbumId, _photo.Id), newValue);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, "Cannot rename photo");
            activity?.AddException(ex);
            ProcessError?.LogError(ex);
        }
    }

    private void CloseDetails()
    {
        using var activity = DiagnosticConfig.StartUserActivity("Thumbnails: Close Details");

        _showDetails = false;
    }

    public async Task OnNewThumbnail(Guid folderId, Guid albumId, Guid id)
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity("Thumbnails: New Thumbnail event");

        var photo = _thumbnails.FirstOrDefault(p => p.FolderId == folderId && p.AlbumId == albumId && p.Id == id);
        if (photo is null)
        {
            return;
        }

        try
        {
            photo.ThumbnailUri = await GetPhotoThumbnailUri.Process(new(folderId, albumId, id));
            StateHasChanged();
        }
        catch (Exception ex)
        {
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, "Cannot get thumbnail URI");
            activity?.AddException(ex);
            ProcessError?.LogError(ex);
        }
    }

    public async Task OnMetadataReady(Guid folderId, Guid albumId, Guid id)
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity("Thumbnails: Metadata Ready event");

        var photo = _thumbnails.FirstOrDefault(p => p.FolderId == folderId && p.AlbumId == albumId && p.Id == id);
        if (photo is null)
        {
            return;
        }

        try
        {
            var actualPhoto = await GetPhoto.Process(new(folderId, albumId, id));

            photo.Metadata = actualPhoto.Metadata ?? new();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, "Cannot get photo metadata");
            activity?.AddException(ex);
            ProcessError?.LogError(ex);
        }
    }

    private void FilterChanged(string newValue)
    {
        using var activity = DiagnosticConfig.StartUserActivity("Thumbnails: Filter Changed event");

        if (string.IsNullOrWhiteSpace(newValue))
        {
            _filter = null;
        }
    }
}