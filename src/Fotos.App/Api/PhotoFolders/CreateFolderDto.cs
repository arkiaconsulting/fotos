using FluentValidation;

namespace Fotos.App.Api.PhotoFolders;

/// <summary>
/// Create a new folder.
/// </summary>
/// <param name="ParentId">The ID of the parent folder</param>
/// <param name="Name" example="Travels">The name of the folder to create</param>
internal readonly record struct CreateFolderDto(Guid ParentId, string Name)
{
    internal sealed class Validator : AbstractValidator<CreateFolderDto>
    {
        public Validator()
        {
            RuleFor(x => x.ParentId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}