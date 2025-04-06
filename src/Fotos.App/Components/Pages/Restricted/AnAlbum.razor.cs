using Fotos.App.Application.Albums;
using Fotos.App.Components.Models;
using Fotos.App.Features;
using Fotos.App.Features.Photos;
using Fotos.App.Hubs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Buffers;

namespace Fotos.App.Components.Pages.Restricted;
public sealed partial class AnAlbum
{
    [Parameter]
    public Guid AlbumId { get; set; }

    [Parameter]
    public Guid FolderId { get; set; }

    [Parameter]
    public Guid ParentId { get; set; }

    [Inject]
    internal GetAlbumBusiness GetAlbum { get; set; } = default!;

    private AlbumModel _album = default!;
    private Thumbnails _thumbnailsComponent = new();
    private string _dragClass = DefaultDragClass;
    private bool _loaded;

    private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full align-content-center";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            RealTimeMessageService.OnThumbnailReady += OnThumbnailReady;
            RealTimeMessageService.OnMetadataReady += OnMetadataReady;
            await RealTimeMessageService.StartAsync();

            var album = await GetAlbum.Process(new(FolderId, AlbumId));
            _album = new AlbumModel { Id = album.Album.Id, FolderId = album.Album.FolderId, Name = album.Album.Name.Value, PhotoCount = album.PhotoCount };

            StateHasChanged();
        }
    }

    private void OnThumbnailReady(object? sender, PhotoId id)
    {
        InvokeAsync(() => _thumbnailsComponent.OnNewThumbnail(id.FolderId, id.AlbumId, id.Id));
    }

    private void OnMetadataReady(object? sender, PhotoId id)
    {
        InvokeAsync(() => _thumbnailsComponent.OnMetadataReady(id.FolderId, id.AlbumId, id.Id));
    }

    private async Task UploadPhotos(IReadOnlyList<IBrowserFile> files)
    {
        await Task.WhenAll(files.Select(UploadSinglePhoto));
    }

    private async Task UploadSinglePhoto(IBrowserFile file)
    {
        if (file.Size > Constants.MaxPhotoSize)
        {
            Snackbar.Add(new MarkupString("<div id='alert'><span>The file is too large.</span></div>"), Severity.Error);

            return;
        }

        if (!Constants.AllowedPhotoContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            Snackbar.Add(new MarkupString("<div id='alert'><span>Only photos can be uploaded.</span></div>"), Severity.Error);

            return;
        }

        byte[] buffer = ArrayPool<byte>.Shared.Rent(512 * 1024);
        await using var stream = file.OpenReadStream(maxAllowedSize: Constants.MaxPhotoSize);

        await using var ms = new MemoryStream();
        int bytesRead;
        long totalBytesRead = 0;
        while ((bytesRead = await stream.ReadAsync(buffer)) != 0)
        {
            totalBytesRead += bytesRead;
            await ms.WriteAsync(buffer.AsMemory(0, bytesRead));
        }

        var id = await AddPhoto(new(FolderId, AlbumId), new(ms.ToArray(), file.ContentType, file.Name));

        var photo = new PhotoModel(FolderId, AlbumId, id, file.Name, new());
        _thumbnailsComponent.AddPhoto(photo);
    }

    private void GoParentFolder()
    {
        NavigationManager.NavigateTo("/");
    }

    private void SetDragClass()
        => _dragClass = $"{DefaultDragClass} mud-border-primary";

    private void ClearDragClass()
        => _dragClass = DefaultDragClass;

    public async ValueTask DisposeAsync()
    {
        await RealTimeMessageService.DisposeAsync();
    }

    private void ThumbnailsLoaded()
    {
        _loaded = true;

        StateHasChanged();
    }

    private void PhotoRemoved()
    {
        StateHasChanged();
    }
}