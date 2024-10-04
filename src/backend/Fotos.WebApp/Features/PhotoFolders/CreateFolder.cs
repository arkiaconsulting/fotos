using FluentValidation;
using Fotos.WebApp.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.WebApp.Features.PhotoFolders;

internal static class EndpointExtension
{
    public static IEndpointRouteBuilder MapPhotoFolderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/folders", ([FromBody] CreateFolder folder) => Results.Ok())
            .AddEndpointFilter<ValidationEndpointFilter>()
            .WithTags("Folders")
            .WithSummary("Create a new folder in an existing folder")
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