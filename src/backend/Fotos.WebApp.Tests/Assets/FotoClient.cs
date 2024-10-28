using System.Net.Mime;
using System.Text;

namespace Fotos.WebApp.Tests.Assets;

internal static class FotoClient
{
    public static async Task<HttpResponseMessage> CreatePhotoFolder(this HttpClient client, Guid parentFolderId, string name)
    {
        var body = $$"""
{
    "parentId":"{{parentFolderId}}",
    "name":"{{name}}"
}
""";

        using var content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);

        return await client.PostAsync(new Uri("api/folders", UriKind.Relative), content);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
    public static async Task<HttpResponseMessage> CreatePhotoFolderWithBody(this HttpClient client, string? body)
    {
        if (body is null)
        {
            return await client.PostAsJsonAsync(new Uri("api/folders", UriKind.Relative), body!);
        }

        var content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);

        return await client.PostAsync(new Uri("api/folders", UriKind.Relative), content);
    }

    public static async Task<HttpResponseMessage> ListPhotoFolders(this HttpClient client, Guid folderId)
    {
        return await client.GetAsync(new Uri($"api/folders/{folderId}/children", UriKind.Relative));
    }

    public static async Task<HttpResponseMessage> GetFolder(this HttpClient client, Guid parentId, Guid folderId)
    {
        return await client.GetAsync(new Uri($"api/folders/{parentId}/{folderId}", UriKind.Relative));
    }

    public static async Task<HttpResponseMessage> RemoveFolder(this HttpClient client, Guid parentId, Guid folderId)
    {
        return await client.DeleteAsync(new Uri($"api/folders/{parentId}/{folderId}", UriKind.Relative));
    }

    public static async Task<HttpResponseMessage> UpdateFolder(this HttpClient client, Guid parentId, Guid folderId, string name)
    {
        var body = $$"""
        {
            "name":"{{name}}"
        }
        """;
        using var content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);
        return await client.PatchAsync(new Uri($"api/folders/{parentId}/{folderId}", UriKind.Relative), content);
    }

    public static async Task<HttpResponseMessage> CreatePhotoAlbum(this HttpClient client, Guid folderId, string name)
    {
        var body = $$"""
{
    "name":"{{name}}"
}
""";

        using var content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);

        return await client.PostAsync(new Uri($"api/folders/{folderId}/albums", UriKind.Relative), content);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
    public static async Task<HttpResponseMessage> CreatePhotoAlbumWithBody(this HttpClient client, Guid folderId, string? body)
    {
        if (body is null)
        {
            return await client.PostAsJsonAsync(new Uri($"api/folders/{folderId}/albums", UriKind.Relative), body!);
        }

        var content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);

        return await client.PostAsync(new Uri($"api/folders/{folderId}/albums", UriKind.Relative), content);
    }

    public static async Task<HttpResponseMessage> AddPhoto(this HttpClient client, Guid folderId, Guid albumId, byte[] photo)
    {
        await using var ms = new MemoryStream(photo);

        using var streamContent = new StreamContent(ms);
        using var content = new MultipartFormDataContent()
        {
            { streamContent, "photo", "photo.jpg" }
        };

        return await client.PostAsync(new Uri($"api/folders/{folderId}/albums/{albumId}/photos", UriKind.Relative), content);
    }

    public static async Task<HttpResponseMessage> ListFolderAlbums(this HttpClient client, Guid folderId)
    {
        return await client.GetAsync(new Uri($"api/folders/{folderId}/albums", UriKind.Relative));
    }

    public static async Task<HttpResponseMessage> GetAlbum(this HttpClient client, Guid folderId, Guid albumId)
    {
        return await client.GetAsync(new Uri($"api/folders/{folderId}/albums/{albumId}", UriKind.Relative));
    }

    public static async Task<HttpResponseMessage> ListPhotos(this HttpClient client, Guid folderId, Guid albumId)
    {
        return await client.GetAsync(new Uri($"api/folders/{folderId}/albums/{albumId}/photos", UriKind.Relative));
    }

    public static async Task<HttpResponseMessage> RemovePhoto(this HttpClient client, Guid folderId, Guid albumId, Guid photoId)
    {
        return await client.DeleteAsync(new Uri($"api/folders/{folderId}/albums/{albumId}/photos/{photoId}", UriKind.Relative));
    }

    public static async Task<HttpResponseMessage> GetOriginalUri(this HttpClient client, Guid folderId, Guid albumId, Guid photoId)
    {
        using var response = await client.GetAsync(new Uri($"api/folders/{folderId}/albums/{albumId}/photos/{photoId}/originaluri", UriKind.Relative));

        return response;
    }

    public static async Task<HttpResponseMessage> GetThumbnailUri(this HttpClient client, Guid folderId, Guid albumId, Guid photoId)
    {
        using var response = await client.GetAsync(new Uri($"api/folders/{folderId}/albums/{albumId}/photos/{photoId}/thumbnailuri", UriKind.Relative));

        return response;
    }

    public static async Task<HttpResponseMessage> UpdatePhoto(this HttpClient client, Guid folderId, Guid albumId, Guid photoId, string title)
    {
        var body = $$"""
{
    "title":"{{title}}"
}
""";
        using var content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);
        return await client.PatchAsync(new Uri($"api/folders/{folderId}/albums/{albumId}/photos/{photoId}", UriKind.Relative), content);
    }
}
