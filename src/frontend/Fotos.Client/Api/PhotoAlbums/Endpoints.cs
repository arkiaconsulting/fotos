using Fotos.Client.Api.Framework;
using Fotos.Client.Api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.Client.Api.PhotoAlbums;

internal static class EndpointExtension
{
    public static IEndpointRouteBuilder MapPhotoAlbumEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/folders/{folderId}/albums", async ([FromRoute] Guid folderId, [FromBody] CreateAlbumDto folder, [FromServices] AddAlbumToStore addAlbum, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("create album", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);

            var newAlbum = new Album(Guid.NewGuid(), folderId, Name.Create(folder.Name));
            await addAlbum(newAlbum);

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("album created"));

            return Results.NoContent();
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("Create a new album in an existing folder")
            .WithTags("Albums")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapGet("api/folders/{folderId}/albums", async ([FromRoute] Guid folderId, [FromServices] GetFolderAlbumsFromStore getFolderAlbums, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("list albums", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);

            var albums = await getFolderAlbums(folderId);

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("albums listed", tags: new(new Dictionary<string, object?>() { ["count"] = albums.Count })));

            return Results.Ok(albums.Select(a => new AlbumDto(a.Id, a.FolderId, a.Name.Value)));
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("List the albums attached to the given folder")
            .WithTags("Albums")
            .Produces<IEnumerable<AlbumDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapGet("api/folders/{folderId:guid}/albums/{albumId:guid}", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromServices] GetAlbumFromStore getAlbum, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("get album", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);
            activity?.SetTag("albumId", albumId);

            var album = await getAlbum(new(folderId, albumId));

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("album retrieved"));

            return Results.Ok(new AlbumDto(album.Id, album.FolderId, album.Name.Value));
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("Get a specific album")
            .WithTags("Albums")
            .Produces<AlbumDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        return endpoints;
    }
}