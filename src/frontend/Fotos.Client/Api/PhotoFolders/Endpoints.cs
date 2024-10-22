using Fotos.Client.Api.Framework;
using Fotos.Client.Api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.Client.Api.PhotoFolders;

internal static class EndpointExtension
{
    public static IEndpointRouteBuilder MapPhotoFolderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/folders", async ([FromBody] CreateFolderDto folder, [FromServices] AddFolderToStore storeNewFolder) =>
        {
            var folderName = Name.Create(folder.Name);
            var id = Guid.NewGuid();
            var newFolder = new Folder(id, folder.ParentId, folderName);

            await storeNewFolder(newFolder);

            return Results.Ok(id);
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithTags("Folders")
            .WithSummary("Create a new folder in an existing folder")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapGet("api/folders/{folderId}/children", async (Guid folderId, [FromServices] GetFoldersFromStore getFolders) =>
        {
            var folders = await getFolders(folderId);

            return Results.Ok(folders.Select(f => new FolderDto(f.Id, f.ParentId, f.Name.Value)));
        })
            .WithTags("Folders")
            .WithSummary("List child folders")
            .Produces<IEnumerable<FolderDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapGet("api/folders/{parentId}/{folderId}", async (Guid parentId, Guid folderId, [FromServices] GetFolderFromStore getFolder) =>
        {
            try
            {
                var folder = await getFolder(parentId, folderId);

                return Results.Ok(new FolderDto(folder.Id, folder.ParentId, folder.Name.Value));
            }
            catch (InvalidOperationException)
            {
                return Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: "The given folder does not exist");
            }
        })
            .WithTags("Folders")
            .WithSummary("Get an existing folder")
            .Produces<FolderDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        endpoints.MapDelete("api/folders/{parentId}/{folderId}", async (Guid parentId, Guid folderId, [FromServices] RemoveFolderFromStore removeFolder) =>
        {
            try
            {
                await removeFolder(parentId, folderId);

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
