using Fotos.Client.Api.Types;

namespace Fotos.Client.Api.Account;

// Store
internal delegate Task<FotoUser?> FindUserInStore(FotoUserId userId);
internal delegate Task AddUserToStore(FotoUser user);
