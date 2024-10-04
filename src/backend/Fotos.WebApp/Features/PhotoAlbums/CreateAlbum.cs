using FluentValidation;
using Fotos.WebApp.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.WebApp.Features.PhotoAlbums;

internal static class EndpointExtension
{
    public static IEndpointRouteBuilder MapPhotoAlbumEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/albums", ([FromBody] CreateAlbum folder) => Results.Ok())
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithSummary("Create a new album in an existing folder")
            .WithTags("Albums")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapPost("api/albums/{albumId:guid}", (Guid albumId, IFormFile photo) => Results.Accepted())
            .DisableAntiforgery()
            .WithSummary("Upload a photo to an existing album")
            .WithTags("Albums")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2212
                operation.Parameters[0].Description = "The ID of the album to upload the photo to";

                return operation;
            });

        return endpoints;
    }
}

/// <summary>
/// Create a new album.
/// </summary>
/// <param name="FolderId">The ID of the parent folder</param>
/// <param name="Name" example="New York 2024">The name of the album to create</param>
internal readonly record struct CreateAlbum(Guid FolderId, string Name)
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
    internal sealed class Validator : AbstractValidator<CreateAlbum>
    {
        public Validator()
        {
            RuleFor(x => x.FolderId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
