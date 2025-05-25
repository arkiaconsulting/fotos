using Fotos.App.Domain;

namespace Fotos.App.Application.User;

internal sealed record FindUserQuery(string Provider, string ProviderUserId) : IQuery<FotoUser?>
{
    public sealed class Handler : IQueryHandler<FindUserQuery, FotoUser?>
    {
        private readonly FindUserInStore _findUserInStore;

        public Handler(FindUserInStore findUserInStore)
        {
            _findUserInStore = findUserInStore;
        }

        async Task<Result<FotoUser?>> IQueryHandler<FindUserQuery, FotoUser?>.Handle(FindUserQuery query, CancellationToken cancellationToken)
        {
            var userId = FotoUserId.Create(query.Provider, query.ProviderUserId);

            return await _findUserInStore(userId);
        }
    }
}
