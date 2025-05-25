namespace Fotos.Application.User;

// Store
public delegate Task<FotoUser?> FindUserInStore(FotoUserId userId);
public delegate Task AddUserToStore(FotoUser user);
