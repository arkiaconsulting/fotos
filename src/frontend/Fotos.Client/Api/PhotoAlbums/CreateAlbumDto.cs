using FluentValidation;

namespace Fotos.Client.Api.PhotoAlbums;

/// <summary>
/// Create a new album.
/// </summary>
/// <param name="Name" example="New York 2024">The name of the album to create</param>
internal readonly record struct CreateAlbumDto(string Name)
{
    internal sealed class Validator : AbstractValidator<CreateAlbumDto>
    {
        public Validator() => RuleFor(x => x.Name).NotEmpty();
    }
}
