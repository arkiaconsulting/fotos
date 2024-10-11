﻿using Fotos.WebApp.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.WebApp.Features.Photos;

internal static class EndpointExtension
{
    public static IEndpointRouteBuilder MapPhotosEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/folders/{folderId:guid}/albums/{albumId:guid}/photos", async ([FromRoute] Guid folderId, [FromRoute] Guid albumId, IFormFile photo, [FromServices] AddPhoto addPhoto) =>
        {
            var aPhoto = new Photo(folderId, albumId, new Uri("https://example.com/photo.jpg"));

            await addPhoto(aPhoto);

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
            var photos = await listPhotos(folderId, albumId);

            return Results.Ok(photos);
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("List the photos of an album")
            .WithTags("Photos")
            .Produces<IEnumerable<Photo>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        return endpoints;
    }
}

/// <summary>
/// A photo in an album.
/// </summary>
/// <param name="FolderId">The ID of the folder that contains the album</param>
/// <param name="AlbumId">The ID of the album that contains the photos</param>
/// <param name="Uri">The URL of the photo file</param>
internal readonly record struct Photo(Guid FolderId, Guid AlbumId, Uri Uri);