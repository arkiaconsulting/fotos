using System.Net.Http.Headers;

namespace Fotos.Client.Features.PhotoFolders;

internal sealed class FotosApiClient
{
    private readonly HttpClient _httpClient;

    public FotosApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IReadOnlyCollection<Folder>> GetFolders(Guid parentId)
    {
        var folders = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<Folder>>($"api/folders/{parentId}/children");

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

    public async Task<Folder> GetFolder(Guid parentId, Guid folderId)
    {
        var folder = await _httpClient.GetFromJsonAsync<Folder>($"api/folders/{parentId}/{folderId}");

        return folder!;
    }

    public async Task RemoveFolder(Guid parentId, Guid folderId)
    {
        using var response = await _httpClient.DeleteAsync(new Uri($"api/folders/{parentId}/{folderId}", UriKind.Relative));

        response.EnsureSuccessStatusCode();
    }

    internal async Task<IReadOnlyCollection<Album>> GetAlbums(Guid folderId)
    {
        var albums = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<Album>>($"api/folders/{folderId}/albums");

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

    internal async Task<Album> GetAlbum(Guid folderId, Guid albumId)
    {
        var album = await _httpClient.GetFromJsonAsync<Album>($"api/folders/{folderId}/albums/{albumId}");

        return album!;
    }

    internal async Task<IReadOnlyCollection<Photo>> ListPhotos(Guid folderId, Guid albumId)
    {
        var photos = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<Photo>>($"api/folders/{folderId}/albums/{albumId}/photos");

        return photos!;
    }

    internal async Task<Guid> AddPhoto(Guid folderId, Guid albumId, PhotoBinary photoBinary)
    {
        await using var ms = new MemoryStream(photoBinary.Buffer.ToArray());
        using var streamContent = new StreamContent(ms);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(photoBinary.ContentType);
        using var content = new MultipartFormDataContent()
        {
            { streamContent, "photo", photoBinary.FileName }
        };

        using var response = await _httpClient.PostAsync(new Uri($"api/folders/{folderId}/albums/{albumId}/photos", UriKind.Relative), content);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Guid>()!;
    }

    internal async Task RemovePhoto(Guid folderId, Guid albumId, Guid photoId)
    {
        using var response = await _httpClient.DeleteAsync(new Uri($"api/folders/{folderId}/albums/{albumId}/photos/{photoId}", UriKind.Relative));

        response.EnsureSuccessStatusCode();
    }

    internal async Task<Uri> GetOriginalUri(Guid folderId, Guid albumId, Guid photoId)
    {
        var uri = await _httpClient.GetFromJsonAsync<Uri>($"api/folders/{folderId}/albums/{albumId}/photos/{photoId}/originaluri");

        return uri!;
    }

    internal async Task<Uri> GetThumbnailUri(Guid folderId, Guid albumId, Guid photoId)
    {
        var uri = await _httpClient.GetFromJsonAsync<Uri>($"api/folders/{folderId}/albums/{albumId}/photos/{photoId}/thumbnailuri");

        return uri!;
    }
}