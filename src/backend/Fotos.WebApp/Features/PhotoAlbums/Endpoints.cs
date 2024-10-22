using Fotos.WebApp.Features.Shared;
using Fotos.WebApp.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.WebApp.Features.PhotoAlbums;

internal static class EndpointExtension
{
    public static IEndpointRouteBuilder MapPhotoAlbumEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/folders/{folderId}/albums", async ([FromRoute] Guid folderId, [FromBody] CreateAlbumDto folder, [FromServices] AddAlbumToStore addAlbum) =>
        {
            var newAlbum = new Album(Guid.NewGuid(), folderId, Name.Create(folder.Name));
            await addAlbum(newAlbum);

            return Results.NoContent();
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("Create a new album in an existing folder")
            .WithTags("Albums")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapGet("api/folders/{folderId}/albums", async ([FromRoute] Guid folderId, [FromServices] GetFolderAlbumsFromStore getFolderAlbums) =>
        {
            var albums = await getFolderAlbums(folderId);

            return Results.Ok(albums.Select(a => new AlbumDto(a.Id, a.FolderId, a.Name.Value)));
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("List the albums attached to the given folder")
            .WithTags("Albums")
            .Produces<IEnumerable<AlbumDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapGet("api/folders/{folderId:guid}/albums/{albumId:guid}", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromServices] GetAlbumFromStore getAlbum) =>
        {
            var album = await getAlbum(new(folderId, albumId));

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