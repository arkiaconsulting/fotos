using Fotos.App.Api.Types;

namespace Fotos.App.Api.Account;

// Store
internal delegate Task<FotoUser?> FindUserInStore(FotoUserId userId);
internal delegate Task AddUserToStore(FotoUser user);
