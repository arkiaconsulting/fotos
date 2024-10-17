namespace Fotos.Client.Components.Models;

internal sealed class PhotoModel
{
    public Uri? OriginalUri { get; set; }
    public Uri ThumbnailUri { get; set; }
    public Guid FolderId { get; }
    public Guid AlbumId { get; }
    public Guid Id { get; }
    public string Title { get; }

    public PhotoModel(Guid folderId, Guid albumId, Guid id, string title)
    {
        FolderId = folderId;
        AlbumId = albumId;
        Id = id;
        Title = title;
        ThumbnailUri = new Uri("img/new.png", UriKind.Relative);
    }
}