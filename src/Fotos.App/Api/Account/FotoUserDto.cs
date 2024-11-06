namespace Fotos.App.Api.Account;

/// <summary>
/// A Foto App user
/// </summary>
/// <param name="GivenName" example="Nick">The name of the user that the Fotos App should use</param>
/// <param name="RootFolderId" example="d775fea4-79ec-4b89-bb7f-06a364d2432c">The ID of the root folder of the user</param>
internal readonly record struct FotoUserDto(string GivenName, Guid RootFolderId);