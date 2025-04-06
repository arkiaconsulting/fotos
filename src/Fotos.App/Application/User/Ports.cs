using Fotos.App.Domain;

namespace Fotos.App.Application.User;

// Store
internal delegate Task<FotoUser?> FindUserInStore(FotoUserId userId);
internal delegate Task AddUserToStore(FotoUser user);
