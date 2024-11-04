using Fotos.Client.Api.Account;

namespace Fotos.Client.Features.Account;

public delegate Task SaveUser(string provider, string userId, string name);
internal delegate Task<FotoUserDto> GetMe();
