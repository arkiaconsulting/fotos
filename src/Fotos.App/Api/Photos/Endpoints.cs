using Fotos.App.Api.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.App.Api.Photos;

internal static class EndpointExtension
{
    public static IEndpointRouteBuilder MapPhotosEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/folders/{folderId:guid}/albums/{albumId:guid}/photos")
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithTags("Photos")
            .RequireAuthorization(Authentication.Constants.ApiPolicy)
            .WithOpenApi();

        group.MapPost("", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, IFormFile photo, [FromServices] AddPhotosBusiness business, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("upload photo", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);
            activity?.SetTag("albumId", albumId);

            await using var stream = photo.OpenReadStream();

            var id = await business.Process(folderId, albumId, stream, photo.ContentType, photo.FileName);

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("photo uploaded", tags: [new("id", id)]));

            instrumentation.PhotoUploadCounter.Add(1);

            return Results.Accepted(value: id.ToString());
        })
            .DisableAntiforgery()
            .WithSummary("Upload a photo to an existing album")
            .Produces<Guid>(StatusCodes.Status202Accepted)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2212
                operation.Parameters[0].Description = "The ID of the folder containing the album";
                operation.Parameters[1].Description = "The ID of the album where to upload the photo to";

                return operation;
            });

        group.MapGet("", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromServices] ListPhotosFromStore listPhotos, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("list photos", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);
            activity?.SetTag("albumId", albumId);

            var photos = await listPhotos(new(folderId, albumId));

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("photos listed", tags: [new("count", photos.Count)]));

            return Results.Ok(photos.Select(p => new PhotoDto(p.Id.Id, p.Id.FolderId, p.Id.AlbumId, p.Title, p.Metadata ?? new())));
        })
            .WithSummary("List the photos of an album")
            .Produces<IEnumerable<PhotoDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapDelete("{id:guid}", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromRoute] Guid id, [FromServices] RemovePhotoBusiness business, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("remove photo", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);
            activity?.SetTag("albumId", albumId);
            activity?.SetTag("photoId", id);

            await business.Process(new(folderId, albumId, id));

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("photo removed"));

            return Results.NoContent();
        })
            .WithSummary("Remove a photo from an album")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("{id:guid}/originaluri", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromRoute] Guid id, [FromServices] GetOriginalStorageUri getOriginalUri, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("get original photo URI", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);
            activity?.SetTag("albumId", albumId);
            activity?.SetTag("photoId", id);

            var uri = await getOriginalUri(new(folderId, albumId, id));

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("original photo URI retrieved"));

            return Results.Ok(uri);
        })
            .WithSummary("Get the original URI of a photo")
            .Produces<Uri>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("{id:guid}/thumbnailuri", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromRoute] Guid id, [FromServices] GetThumbnailStorageUri getThumbnailUri, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("get thumbnail photo URI", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);
            activity?.SetTag("albumId", albumId);
            activity?.SetTag("photoId", id);

            var uri = await getThumbnailUri(new(folderId, albumId, id));

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("thumbnail photo URI retrieved"));

            return Results.Ok(uri);
        })
            .WithSummary("Get the thumbnail URI of a photo")
            .Produces<Uri>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPatch("{id:guid}", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromRoute] Guid id, [FromBody] Photo photo, [FromServices] UpdatePhotoBusiness business, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("update photo", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);
            activity?.SetTag("albumId", albumId);
            activity?.SetTag("photoId", id);

            await business.Process(new(folderId, albumId, id), photo.Title);

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("photo updated"));

            return Results.NoContent();
        })
            .WithSummary("Modify an existing photo")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("{id:guid}", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, [FromRoute] Guid id, [FromServices] GetPhotoBusiness business, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("retrieve a photo", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);
            activity?.SetTag("albumId", albumId);
            activity?.SetTag("photoId", id);

            var photo = await business.Process(new(folderId, albumId, id));

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("photo retrieved"));

            return Results.Ok(new PhotoDto(photo.Id.Id, photo.Id.FolderId, photo.Id.AlbumId, photo.Title, photo.Metadata ?? new()));
        })
            .WithSummary("Remove a photo from an album")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return endpoints;
    }
}
