using FluentValidation;

namespace Fotos.Client.Api.Account;

/// <summary>
/// Create a new user
/// </summary>
/// <param name="GivenName" example="Nick">The name of the user to be use by the Foto App</param>
internal readonly record struct CreateFotoUserDto(string GivenName)
{
    internal sealed class Validator : AbstractValidator<CreateFotoUserDto>
    {
        public Validator()
        {
            RuleFor(x => x.GivenName).NotEmpty();
        }
    }
}
