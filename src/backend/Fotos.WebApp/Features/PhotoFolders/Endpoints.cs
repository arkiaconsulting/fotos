using FluentValidation;
using Fotos.WebApp.Framework;
using Fotos.WebApp.Types;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.WebApp.Features.PhotoFolders;

internal static class EndpointExtension
{
    public static IEndpointRouteBuilder MapPhotoFolderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/folders", async ([FromBody] CreateFolder folder, [FromServices] StoreNewFolder storeNewFolder) =>
        {
            var folderName = Name.Create(folder.Name);
            var newFolder = new Folder(Guid.NewGuid(), folder.ParentId, folderName);

            await storeNewFolder(newFolder);

            return Results.NoContent();
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithTags("Folders")
            .WithSummary("Create a new folder in an existing folder")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapGet("api/folders/{folderId}/children", async (Guid folderId, [FromServices] GetFolders getFolders) =>
        {
            var folders = await getFolders(folderId);

            return Results.Ok(folders);
        })
            .WithTags("Folders")
            .WithSummary("List child folders")
            .Produces<IEnumerable<Folder>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapGet("api/folders/{folderId}", async (Guid folderId, [FromServices] GetFolder getFolder) =>
        {
            try
            {
                var folder = await getFolder(folderId);

                return Results.Ok(folder);
            }
            catch (InvalidOperationException)
            {
                return Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: "The given folder does not exist");
            }
        })
            .WithTags("Folders")
            .WithSummary("Get an existing folder")
            .Produces<Folder>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapDelete("api/folders/{folderId}", async (Guid folderId, [FromServices] RemoveFolder removeFolder) =>
        {
            try
            {
                await removeFolder(folderId);

                return Results.NoContent();
            }
            catch (InvalidOperationException)
            {
                return Results.NoContent();
            }
        })
            .WithTags("Folders")
            .WithSummary("Remove an existing folder")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        return endpoints;
    }
}

/// <summary>
/// Create a new folder.
/// </summary>
/// <param name="ParentId">The ID of the parent folder</param>
/// <param name="Name" example="Travels">The name of the folder to create</param>
internal readonly record struct CreateFolder(Guid ParentId, string Name)
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
    internal sealed class Validator : AbstractValidator<CreateFolder>
    {
        public Validator()
        {
            RuleFor(x => x.ParentId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}