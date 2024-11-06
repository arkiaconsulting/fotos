namespace Fotos.App.Api.Photos;

/// <summary>
/// A photo in an album.
/// </summary>
/// <param name="Id">The ID of the photo</param>
/// <param name="FolderId">The ID of the folder that contains the album</param>
/// <param name="AlbumId">The ID of the album that contains the photos</param>
/// <param name="Title">The title of the photo</param>
/// <param name="Metadata">Additional metadata contained into the original photo</param>
internal readonly record struct PhotoDto(Guid Id, Guid FolderId, Guid AlbumId, string Title, ExifMetadata Metadata);