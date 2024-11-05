using Fotos.Client.Api.Framework;
using Fotos.Client.Api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.Client.Api.PhotoFolders;

internal static class EndpointExtension
{
    public static IEndpointRouteBuilder MapPhotoFolderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/folders")
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithTags("Folders")
            .RequireAuthorization(Authentication.Constants.DefaultPolicy)
            .WithOpenApi();

        group.MapPost("", async ([FromBody] CreateFolderDto folder, [FromServices] AddFolderToStore storeNewFolder, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("create folder", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("parentFolderId", folder.ParentId);

            var folderName = Name.Create(folder.Name);
            var id = Guid.NewGuid();
            var newFolder = new Folder(id, folder.ParentId, folderName);

            await storeNewFolder(newFolder);

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("folder created", tags: new(new Dictionary<string, object?> { ["id"] = id })));

            return Results.Ok(id);
        })
            .WithSummary("Create a new folder in an existing folder")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("{folderId}/children", async (Guid folderId, [FromServices] GetFoldersFromStore getFolders, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("list child folders", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);

            var folders = await getFolders(folderId);

            activity?.AddEvent(new System.Diagnostics.ActivityEvent("folders listed", tags: new(new Dictionary<string, object?> { ["count"] = folders.Count })));

            return Results.Ok(folders.Select(f => new FolderDto(f.Id, f.ParentId, f.Name.Value)));
        })
            .WithSummary("List child folders")
            .Produces<IEnumerable<FolderDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("{parentId}/{folderId}", async (Guid parentId, Guid folderId, [FromServices] GetFolderFromStore getFolder, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("get folder", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);
            activity?.SetTag("parentFolderId", parentId);

            try
            {
                var folder = await getFolder(parentId, folderId);

                activity?.AddEvent(new System.Diagnostics.ActivityEvent("folder retrieved"));

                return Results.Ok(new FolderDto(folder.Id, folder.ParentId, folder.Name.Value));
            }
            catch (InvalidOperationException)
            {
                activity?.AddEvent(new System.Diagnostics.ActivityEvent("folder not found"));

                return Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: "The given folder does not exist");
            }
        })
            .WithSummary("Get an existing folder")
            .Produces<FolderDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapDelete("{parentId}/{folderId}", async (Guid parentId, Guid folderId, [FromServices] RemoveFolderFromStore removeFolder, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("delete folder", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);
            activity?.SetTag("parentFolderId", parentId);

            try
            {
                await removeFolder(parentId, folderId);

                activity?.AddEvent(new System.Diagnostics.ActivityEvent("folder deleted"));
                return Results.NoContent();
            }
            catch (InvalidOperationException)
            {
                activity?.AddEvent(new System.Diagnostics.ActivityEvent("folder not found"));

                return Results.NoContent();
            }
        })
            .WithSummary("Remove an existing folder")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPatch("{parentId}/{folderId}", async (Guid parentId, Guid folderId, [FromBody] UpdateFolderDto folder, [FromServices] UpdateFolderInStore updateFolder, [FromServices] InstrumentationConfig instrumentation) =>
        {
            using var activity = instrumentation.ActivitySource.StartActivity("update folder", System.Diagnostics.ActivityKind.Server);
            activity?.SetTag("folderId", folderId);
            activity?.SetTag("parentFolderId", parentId);

            var folderName = Name.Create(folder.Name);
            try
            {
                await updateFolder(parentId, folderId, folderName);

                activity?.AddEvent(new System.Diagnostics.ActivityEvent("folder updated"));

                return Results.NoContent();
            }
            catch (InvalidOperationException)
            {
                activity?.AddEvent(new System.Diagnostics.ActivityEvent("folder not found"));

                return Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: "The given folder does not exist");
            }
        })
            .WithSummary("Update an existing folder")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return endpoints;
    }
}
