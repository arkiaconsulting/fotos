using Fotos.Client.Api.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.Client.Api.Photos;

internal static class EndpointExtension
{
    public static IEndpointRouteBuilder MapPhotosEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/folders/{folderId:guid}/albums/{albumId:guid}/photos", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, IFormFile photo, [FromServices] AddPhotosBusiness business) =>
        {
            await using var stream = photo.OpenReadStream();

            var id = await business.Process(folderId, albumId, stream, photo.ContentType, photo.FileName);

            return Results.Accepted(value: id.ToString());
        })
            .DisableAntiforgery()
            .WithSummary("Upload a photo to an existing album")
            .WithTags("Photos")
            .Produces<Guid>(StatusCodes.Status202Accepted)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2212
                operation.Parameters[0].Description = "The ID of the folder containing the album";
                operation.Parameters[1].Description = "The ID of the album where to upload the photo to";

                return operation;
            });

        endpoints.MapGet("api/folders/{folderId:guid}/albums/{albumId:guid}/photos", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromServices] ListPhotosFromStore listPhotos) =>
        {
            var photos = await listPhotos(new(folderId, albumId));

            return Results.Ok(photos.Select(p => new PhotoDto(p.Id.Id, p.Id.FolderId, p.Id.AlbumId, p.Title)));
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("List the photos of an album")
            .WithTags("Photos")
            .Produces<IEnumerable<PhotoDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapDelete("api/folders/{folderId:guid}/albums/{albumId:guid}/photos/{id:guid}", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromRoute] Guid id, [FromServices] RemovePhotoBusiness business) =>
        {
            await business.Process(new(folderId, albumId, id));

            return Results.NoContent();
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("Remove a photo from an album")
            .WithTags("Photos")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapGet("api/folders/{folderId:guid}/albums/{albumId:guid}/photos/{id:guid}/originaluri", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromRoute] Guid id, [FromServices] GetOriginalStorageUri getOriginalUri) =>
        {
            var uri = await getOriginalUri(new(folderId, albumId, id));

            return Results.Ok(uri);
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("Get the original URI of a photo")
            .WithTags("Photos")
            .Produces<Uri>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapGet("api/folders/{folderId:guid}/albums/{albumId:guid}/photos/{id:guid}/thumbnailuri", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromRoute] Guid id, [FromServices] GetThumbnailStorageUri getThumbnailUri) =>
        {
            var uri = await getThumbnailUri(new(folderId, albumId, id));

            return Results.Ok(uri);
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("Get the thumbnail URI of a photo")
            .WithTags("Photos")
            .Produces<Uri>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapPatch("api/folders/{folderId:guid}/albums/{albumId:guid}/photos/{id:guid}", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromRoute] Guid id, [FromBody] Photo photo, [FromServices] UpdatePhotoBusiness business) =>
        {
            await business.Process(new(folderId, albumId, id), photo.Title);

            return Results.NoContent();
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("Modify an existing photo")
            .WithTags("Photos")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        return endpoints;
    }
}
