namespace Fotos.Client.Features.PhotoFolders;

internal sealed class FotosApiClient
{
    private readonly HttpClient _httpClient;

    public FotosApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IReadOnlyCollection<Folder>> GetFolders(Guid folderId)
    {
        var folders = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<Folder>>($"api/folders/{folderId}/children");

        return folders!;
    }

    public async Task CreateFolder(Guid parentId, string name)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/folders", new
        {
            parentId,
            name
        });

        response.EnsureSuccessStatusCode();
    }

    public async Task<Folder> GetFolder(Guid folderId)
    {
        var folder = await _httpClient.GetFromJsonAsync<Folder>($"api/folders/{folderId}");

        return folder!;
    }

    public async Task RemoveFolder(Guid folderId)
    {
        using var response = await _httpClient.DeleteAsync(new Uri($"api/folders/{folderId}", UriKind.Relative));

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
}