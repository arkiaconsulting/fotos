using FluentValidation;

namespace Fotos.WebApp.Features.PhotoFolders;

/// <summary>
/// Create a new folder.
/// </summary>
/// <param name="ParentId">The ID of the parent folder</param>
/// <param name="Name" example="Travels">The name of the folder to create</param>
internal readonly record struct CreateFolderDto(Guid ParentId, string Name)
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
    internal sealed class Validator : AbstractValidator<CreateFolderDto>
    {
        public Validator()
        {
            RuleFor(x => x.ParentId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}