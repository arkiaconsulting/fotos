using Fotos.Client.Api.PhotoAlbums;
using Fotos.Client.Api.PhotoFolders;
using Fotos.Client.Api.Photos;
using Fotos.Client.Features.PhotoAlbums;
using Fotos.Client.Features.Photos;
using System.Net.Http.Headers;

namespace Fotos.Client.Adapters;

internal sealed class FotosApiClient
{
    private readonly HttpClient _httpClient;

    public FotosApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IReadOnlyCollection<FolderDto>> GetFolders(Guid parentId)
    {
        var folders = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<FolderDto>>($"api/folders/{parentId}/children");

        return folders!;
    }

    public async Task<Guid> CreateFolder(Guid parentId, string name)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/folders", new
        {
            parentId,
            name
        });

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Guid>()!;
    }

    public async Task<FolderDto> GetFolder(Guid parentId, Guid folderId)
    {
        var folder = await _httpClient.GetFromJsonAsync<FolderDto>($"api/folders/{parentId}/{folderId}");

        return folder!;
    }

    public async Task RemoveFolder(Guid parentId, Guid folderId)
    {
        using var response = await _httpClient.DeleteAsync(new Uri($"api/folders/{parentId}/{folderId}", UriKind.Relative));

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateFolder(Guid parentId, Guid folderId, string name)
    {
        using var response = await _httpClient.PatchAsJsonAsync(new Uri($"api/folders/{parentId}/{folderId}", UriKind.Relative), new
        {
            name
        });

        response.EnsureSuccessStatusCode();
    }

    internal async Task<IReadOnlyCollection<AlbumDto>> GetAlbums(Guid folderId)
    {
        var albums = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<AlbumDto>>($"api/folders/{folderId}/albums");

        return albums!;
    }

    internal async Task CreateAlbum(Guid folderId, string name)
    {
        using var response = await _httpClient.PostAsJsonAsync($"api/folders/{folderId}/albums", new
        {
            name
        });

        response.EnsureSuccessStatusCode();
    }

    internal async Task<AlbumDto> GetAlbum(AlbumId albumId)
    {
        var album = await _httpClient.GetFromJsonAsync<AlbumDto>($"api/folders/{albumId.FolderId}/albums/{albumId.Id}");

        return album!;
    }

    internal async Task<IReadOnlyCollection<PhotoDto>> ListPhotos(AlbumId albumId)
    {
        var photos = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<PhotoDto>>($"api/folders/{albumId.FolderId}/albums/{albumId.Id}/photos");

        return photos!;
    }

    internal async Task<Guid> AddPhoto(AlbumId albumId, PhotoToUpload photo)
    {
        await using var ms = new MemoryStream(photo.Buffer.ToArray());
        using var streamContent = new StreamContent(ms);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(photo.ContentType);
        using var content = new MultipartFormDataContent()
        {
            { streamContent, "photo", photo.FileName }
        };

        using var response = await _httpClient.PostAsync(new Uri($"api/folders/{albumId.FolderId}/albums/{albumId.Id}/photos", UriKind.Relative), content);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Guid>()!;
    }

    internal async Task RemovePhoto(PhotoId photoId)
    {
        using var response = await _httpClient.DeleteAsync(new Uri($"api/folders/{photoId.FolderId}/albums/{photoId.AlbumId}/photos/{photoId.Id}", UriKind.Relative));

        response.EnsureSuccessStatusCode();
    }

    internal async Task<Uri> GetOriginalUri(PhotoId photoId)
    {
        var uri = await _httpClient.GetFromJsonAsync<Uri>($"api/folders/{photoId.FolderId}/albums/{photoId.AlbumId}/photos/{photoId.Id}/originaluri");

        return uri!;
    }

    internal async Task<Uri> GetThumbnailUri(PhotoId photoId)
    {
        var uri = await _httpClient.GetFromJsonAsync<Uri>($"api/folders/{photoId.FolderId}/albums/{photoId.AlbumId}/photos/{photoId.Id}/thumbnailuri");

        return uri!;
    }

    internal async Task UpdatePhoto(PhotoId photoId, string title)
    {
        using var response = await _httpClient.PatchAsJsonAsync(new Uri($"api/folders/{photoId.FolderId}/albums/{photoId.AlbumId}/photos/{photoId.Id}", UriKind.Relative), new
        {
            title
        });

        response.EnsureSuccessStatusCode();
    }

    internal async Task<PhotoDto> GetPhoto(PhotoId photoId)
    {
        var photo = await _httpClient.GetFromJsonAsync<PhotoDto>($"api/folders/{photoId.FolderId}/albums/{photoId.AlbumId}/photos/{photoId.Id}");

        return photo!;
    }

    internal async Task SaveUser(string provider, string providerUserId, string givenName)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/users", new
        {
            provider,
            providerUserId,
            givenName
        });

        response.EnsureSuccessStatusCode();
    }
}