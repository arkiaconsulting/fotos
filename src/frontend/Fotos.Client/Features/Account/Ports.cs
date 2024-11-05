using Fotos.Client.Api.Account;

namespace Fotos.Client.Features.Account;

public delegate Task SaveUser(string name);
internal delegate Task<FotoUserDto> GetMe();
