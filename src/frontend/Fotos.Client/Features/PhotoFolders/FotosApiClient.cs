namespace Fotos.Client.Features.PhotoFolders;

internal sealed class FotosApiClient
{
    private readonly HttpClient _httpClient;

    public FotosApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IReadOnlyCollection<Folder>> GetFolders(Guid parentId)
    {
        var folders = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<Folder>>($"api/folders/{parentId}");

        return folders!;
    }

    public async Task CreateFolder(Guid parentFolderId, string name)
    {
        var response = await _httpClient.PostAsJsonAsync("api/folders", new
        {
            parentFolderId,
            name
        });

        response.EnsureSuccessStatusCode();
    }
}
