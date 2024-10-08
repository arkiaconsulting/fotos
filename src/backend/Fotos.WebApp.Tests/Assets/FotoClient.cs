﻿using System.Net.Mime;
using System.Text;

namespace Fotos.WebApp.Tests.Assets;

internal static class FotoClient
{
    public static async Task<HttpResponseMessage> CreatePhotoFolder(this HttpClient client, Guid parentFolderId, string name)
    {
        var body = $$"""
{
    "parentFolderId":"{{parentFolderId}}",
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

    public static async Task<HttpResponseMessage> ListPhotoFolders(this HttpClient client, Guid parentFolderId)
    {
        return await client.GetAsync(new Uri($"api/folders/{parentFolderId}", UriKind.Relative));
    }

    public static async Task<HttpResponseMessage> CreatePhotoAlbum(this HttpClient client, Guid folderId, string name)
    {
        var body = $$"""
{
    "folderId":"{{folderId}}",
    "name":"{{name}}"
}
""";

        using var content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);

        return await client.PostAsync(new Uri("api/albums", UriKind.Relative), content);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
    public static async Task<HttpResponseMessage> CreatePhotoAlbumWithBody(this HttpClient client, string? body)
    {
        if (body is null)
        {
            return await client.PostAsJsonAsync(new Uri("api/albums", UriKind.Relative), body!);
        }

        var content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);

        return await client.PostAsync(new Uri("api/albums", UriKind.Relative), content);
    }

    public static async Task<HttpResponseMessage> AddPhoto(this HttpClient client, Guid albumId, byte[] photo)
    {
        await using var ms = new MemoryStream(photo);

        using var streamContent = new StreamContent(ms);
        using var content = new MultipartFormDataContent()
        {
            { streamContent, "photo", "photo.jpg" }
        };

        return await client.PostAsync(new Uri($"api/albums/{albumId}", UriKind.Relative), content);
    }
}
