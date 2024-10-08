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
            var newFolder = new Folder(folder.ParentFolderId, folderName);

            await storeNewFolder(newFolder);

            return Results.Ok();
        })
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithTags("Folders")
            .WithSummary("Create a new folder in an existing folder")
            .WithOpenApi();

        endpoints.MapGet("api/folders/{parentFolderId}", async (Guid parentFolderId, [FromServices] GetFolders getFolders) =>
        {
            var folders = await getFolders(parentFolderId);

            return Results.Ok(folders);
        })
            .WithTags("Folders")
            .WithSummary("List child folders")
            .WithOpenApi();

        return endpoints;
    }
}

/// <summary>
/// Create a new folder.
/// </summary>
/// <param name="ParentFolderId">The ID of the parent folder</param>
/// <param name="Name" example="Travels">The name of the folder to create</param>
internal readonly record struct CreateFolder(Guid ParentFolderId, string Name)
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
    internal sealed class Validator : AbstractValidator<CreateFolder>
    {
        public Validator()
        {
            RuleFor(x => x.ParentFolderId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}