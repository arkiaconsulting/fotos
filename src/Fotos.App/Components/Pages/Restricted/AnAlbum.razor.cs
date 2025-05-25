using Fotos.App.Adapters.RealTimeMessaging;
using Fotos.App.Components.Models;
using Fotos.Application;
using Fotos.Application.Albums;
using Fotos.Application.Photos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Buffers;

namespace Fotos.App.Components.Pages.Restricted;
public sealed partial class AnAlbum
{
    [CascadingParameter]
    public ProcessError? ProcessError { get; set; }

    [Parameter]
    public Guid AlbumId { get; set; }

    [Parameter]
    public Guid FolderId { get; set; }

    [Parameter]
    public Guid ParentId { get; set; }

    [Inject]
    internal ISender Sender { get; set; } = default!;

    private AlbumModel _album = default!;
    private Thumbnails _thumbnailsComponent = new();
    private string _dragClass = DefaultDragClass;
    private bool _loaded;

    private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full align-content-center";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            using var activity = DiagnosticConfig.StartUserActivity("AnAlbum: Initialize");

            try
            {
                RealTimeMessageService.OnThumbnailReady += OnThumbnailReady;
                RealTimeMessageService.OnMetadataReady += OnMetadataReady;
                await RealTimeMessageService.StartAsync();

                var result = await Sender.Send(new GetAlbumQuery(new(FolderId, AlbumId)));
                var album = result.Value;

                _album = new AlbumModel { Id = album.Album.Id, FolderId = album.Album.FolderId, Name = album.Album.Name.Value, PhotoCount = album.PhotoCount };

                StateHasChanged();
            }
            catch (Exception ex)
            {
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, "Cannot initialize AnAlbum page");
                activity?.AddException(ex);
                ProcessError?.LogError(ex);
            }
        }
    }

    private void OnThumbnailReady(object? sender, PhotoId id)
    {
        using var activity = DiagnosticConfig.StartUserActivity("AnAlbum: On new thumbnail ready");

        InvokeAsync(() => _thumbnailsComponent.OnNewThumbnail(id.FolderId, id.AlbumId, id.Id));
    }

    private void OnMetadataReady(object? sender, PhotoId id)
    {
        using var activity = DiagnosticConfig.StartUserActivity("AnAlbum: On metadata ready");

        InvokeAsync(() => _thumbnailsComponent.OnMetadataReady(id.FolderId, id.AlbumId, id.Id));
    }

    private async Task UploadPhotos(IReadOnlyList<IBrowserFile> files)
    {
        using var activity = DiagnosticConfig.StartUserActivity("AnAlbum: upload multiple photos");

        try
        {
            await Task.WhenAll(files.Select(UploadSinglePhoto));
        }
        catch (Exception ex)
        {
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, "Cannot upload photos");
            activity?.AddException(ex);
            ProcessError?.LogError(ex);
        }
    }

    private async Task UploadSinglePhoto(IBrowserFile file)
    {
        using var activity = DiagnosticConfig.AppActivitySource.StartActivity("AnAlbum: upload single photo");
        activity?.SetTag("file.name", file.Name);

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
        ms.Position = 0;

        var command = new AddPhotoCommand(FolderId, AlbumId, ms, file.ContentType, file.Name);

        var result = await Sender.Send(command, CancellationToken.None);

        var photo = new PhotoModel(FolderId, AlbumId, Guid.NewGuid(), file.Name, new());
        _thumbnailsComponent.AddPhoto(photo);
    }

    private void GoParentFolder()
    {
        using var activity = DiagnosticConfig.StartUserActivity("AnAlbum: Go to parent folder");

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
        using var activity = DiagnosticConfig.StartUserActivity("AnAlbum: Thumbnails loaded");

        _loaded = true;

        StateHasChanged();
    }

    private void PhotoRemoved()
    {
        using var activity = DiagnosticConfig.StartUserActivity("AnAlbum: Photo removed");

        StateHasChanged();
    }
}