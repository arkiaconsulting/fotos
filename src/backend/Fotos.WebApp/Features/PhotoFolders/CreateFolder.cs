using FluentValidation;
using Fotos.WebApp.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.WebApp.Features.PhotoFolders;

internal static class EndpointExtension
{
    public static IEndpointRouteBuilder MapPhotoFolderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/folders", ([FromBody] CreateFolder folder) => Results.Ok())
            .AddEndpointFilter<ValidationEndpointFilter>();

        return endpoints;
    }
}

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