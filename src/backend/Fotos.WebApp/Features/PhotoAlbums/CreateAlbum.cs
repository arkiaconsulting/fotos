using FluentValidation;
using Fotos.WebApp.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Fotos.WebApp.Features.PhotoAlbums;

internal static class EndpointExtension
{
    public static IEndpointRouteBuilder MapPhotoAlbumEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/albums", ([FromBody] CreateAlbum folder) => Results.Ok())
            .AddEndpointFilter<ValidationEndpointFilter>();

        return endpoints;
    }
}

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
