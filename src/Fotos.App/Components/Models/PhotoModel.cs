using Fotos.App.Domain;

namespace Fotos.App.Components.Models;

internal sealed class PhotoModel
{
    public Uri? OriginalUri { get; set; }
    public Uri ThumbnailUri { get; set; }
    public Guid FolderId { get; }
    public Guid AlbumId { get; }
    public Guid Id { get; }
    public string Title { get; set; }
    public ExifMetadata Metadata { get; set; }

    public PhotoModel(Guid folderId, Guid albumId, Guid id, string title, ExifMetadata metadata)
    {
        FolderId = folderId;
        AlbumId = albumId;
        Id = id;
        Title = title;
        Metadata = metadata;
        ThumbnailUri = new Uri("img/new.png", UriKind.Relative);
    }

    public static PhotoModel Default() => new(Guid.Empty, Guid.Empty, Guid.Empty, string.Empty, new());
}