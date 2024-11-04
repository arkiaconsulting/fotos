using FluentValidation;

namespace Fotos.Client.Api.Account;

/// <summary>
/// Create a new user
/// </summary>
/// <param name="Provider" example="google">The name of the identity provider</param>
/// <param name="ProviderUserId" example="13283843502303892304589">The ID of the user as per the identity provider</param>
/// <param name="GivenName" example="Nick">The name of the user to be use by the Foto App</param>
internal readonly record struct CreateFotoUserDto(string Provider, string ProviderUserId, string GivenName)
{
    internal sealed class Validator : AbstractValidator<CreateFotoUserDto>
    {
        public Validator()
        {
            RuleFor(x => x.Provider).NotEmpty();
            RuleFor(x => x.ProviderUserId).NotEmpty();
            RuleFor(x => x.GivenName).NotEmpty();
        }
    }
}
