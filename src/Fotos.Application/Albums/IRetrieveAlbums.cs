namespace Fotos.Application.Albums;

internal interface IRetrieveAlbums
{
    Task<IReadOnlyCollection<Album>> ListFolderAlbums(Guid folderId);
    Task<Album> GetAlbum(AlbumId albumId);
    Task<int> GetAlbumPhotoCount(Guid folderId, Guid albumId);
}
