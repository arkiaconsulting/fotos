using Fotos.App.Api.Framework;
using Fotos.App.Api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.App.Api.PhotoAlbums;

internal static class EndpointExtension
{
    public static IEndpointRouteBuilder MapPhotoAlbumEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/folders/{folderId:guid}/albums")
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithTags("Albums")
            .RequireAuthorization(Authentication.Constants.ApiPolicy)
            .WithOpenApi();

        group.MapPost("", async ([FromRoute] Guid folderId, [FromBody] CreateAlbumDto folder, [FromServices] AddAlbumToStore addAlbum, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("create album", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);

            var newAlbum = new Album(Guid.NewGuid(), folderId, Name.Create(folder.Name));
            await addAlbum(newAlbum);

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("album created"));

            return Results.NoContent();
        })
            .WithSummary("Create a new album in an existing folder")
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("", async ([FromRoute] Guid folderId, [FromServices] GetFolderAlbumsBusiness getFolderAlbumsBusiness, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("list albums", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);

            var albums = await getFolderAlbumsBusiness.Process(folderId);

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("albums listed", tags: [new("count", albums.Count)]));

            return Results.Ok(albums
                .Select(x =>
                 {
                     var (album, photoCount) = x;
                     return new AlbumDto(album.Id, album.FolderId, album.Name.Value, photoCount);
                 })
            );
        })
            .WithSummary("List the albums attached to the given folder")
            .Produces<IEnumerable<AlbumDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("{albumId:guid}", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromServices] GetAlbumBusiness getAlbum, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("get album", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);
            activity?.SetTag("albumId", albumId);

            var (album, photoCount) = await getAlbum.Process(new(folderId, albumId));

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("album retrieved"));

            return Results.Ok(new AlbumDto(album.Id, album.FolderId, album.Name.Value, photoCount));
        })
            .WithSummary("Get a specific album")
            .Produces<AlbumDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return endpoints;
    }
}