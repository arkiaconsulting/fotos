using Fotos.WebApp.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.WebApp.Features.Photos;

internal static class EndpointExtension
{
    public static IEndpointRouteBuilder MapPhotosEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/folders/{folderId:guid}/albums/{albumId:guid}/photos", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, IFormFile photo, [FromServices] AddPhotosBusiness business) =>
        {
            await using var stream = photo.OpenReadStream();

            await business.Process(folderId, albumId, stream);

            return Results.Accepted();
        })
            .DisableAntiforgery()
            .WithSummary("Upload a photo to an existing album")
            .WithTags("Photos")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2212
                operation.Parameters[0].Description = "The ID of the album to upload the photo to";

                return operation;
            });

        endpoints.MapGet("api/folders/{folderId:guid}/albums/{albumId:guid}/photos", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromServices] ListPhotos listPhotos) =>
        {
            var photos = await listPhotos(new(folderId, albumId));

            return Results.Ok(photos.Select(e => e.Id));
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("List the photos of an album")
            .WithTags("Photos")
            .Produces<IEnumerable<Photo>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapDelete("api/folders/{folderId:guid}/albums/{albumId:guid}/photos/{id:guid}", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromRoute] Guid id, [FromServices] RemovePhoto removePhoto) =>
        {
            await removePhoto(new(folderId, albumId, id));

            return Results.NoContent();
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("Remove a photo from an album")
            .WithTags("Photos")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        return endpoints;
    }
}

/// <summary>
/// A photo in an album.
/// </summary>
/// <param name="Id">The ID of the photo</param>
/// <param name="FolderId">The ID of the folder that contains the album</param>
/// <param name="AlbumId">The ID of the album that contains the photos</param>
internal readonly record struct Photo(Guid Id, Guid FolderId, Guid AlbumId);