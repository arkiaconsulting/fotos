using FluentValidation;

namespace Fotos.WebApp.Features.PhotoAlbums;

/// <summary>
/// Create a new album.
/// </summary>
/// <param name="Name" example="New York 2024">The name of the album to create</param>
internal readonly record struct CreateAlbumDto(string Name)
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
    internal sealed class Validator : AbstractValidator<CreateAlbumDto>
    {
        public Validator() => RuleFor(x => x.Name).NotEmpty();
    }
}
