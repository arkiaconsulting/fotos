using FluentValidation;
using Fotos.WebApp.Features.Shared;
using Fotos.WebApp.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.WebApp.Features.PhotoAlbums;

internal static class EndpointExtension
{
    public static IEndpointRouteBuilder MapPhotoAlbumEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/folders/{folderId}/albums", async ([FromRoute] Guid folderId, [FromBody] Endpoints folder, [FromServices] AddAlbum addAlbum) =>
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

        endpoints.MapGet("api/folders/{folderId}/albums", async ([FromRoute] Guid folderId, [FromServices] GetFolderAlbums getFolderAlbums) =>
        {
            var albums = await getFolderAlbums(folderId);

            return Results.Ok(albums);
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("List the albums attached to the given folder")
            .WithTags("Albums")
            .Produces<IEnumerable<Album>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapGet("api/folders/{folderId:guid}/albums/{albumId:guid}", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromServices] GetAlbum getAlbum) =>
        {
            var album = await getAlbum(new(folderId, albumId));

            return Results.Ok(album);
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("Get a specific album")
            .WithTags("Albums")
            .Produces<Album>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        return endpoints;
    }
}

/// <summary>
/// Create a new album.
/// </summary>
/// <param name="Name" example="New York 2024">The name of the album to create</param>
internal readonly record struct Endpoints(string Name)
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
    internal sealed class Validator : AbstractValidator<Endpoints>
    {
        public Validator() => RuleFor(x => x.Name).NotEmpty();
    }
}
