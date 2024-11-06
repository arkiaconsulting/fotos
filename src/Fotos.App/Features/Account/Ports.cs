using Fotos.App.Api.Account;

namespace Fotos.App.Features.Account;

public delegate Task SaveUser(string name);
internal delegate Task<FotoUserDto> GetMe();
