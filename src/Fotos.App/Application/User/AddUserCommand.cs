using Fotos.App.Application.Folders;
using Fotos.App.Domain;

namespace Fotos.App.Application.User;

internal sealed record AddUserCommand(FotoUserId UserId, string GivenName) : ICommand
{
    internal sealed class Handler : ICommandHandler<AddUserCommand>
    {
        private readonly FindUserInStore _findUserInStore;
        private readonly AddUserToStore _addUserToStore;
        private readonly AddFolderToStore _addFolderToStore;

        public Handler(FindUserInStore findUserInStore,
            AddUserToStore addUserToStore,
            AddFolderToStore addFolderToStore)
        {
            _findUserInStore = findUserInStore;
            _addUserToStore = addUserToStore;
            _addFolderToStore = addFolderToStore;
        }

        public async Task<Result> Handle(AddUserCommand command, CancellationToken cancellationToken)
        {
            using var activity = DiagnosticConfig.AppActivitySource.StartActivity("BUSINESS: add user", System.Diagnostics.ActivityKind.Internal);

            var (userId, givenName) = command;
            var existingUser = await _findUserInStore(userId);

            if (existingUser is not null)
            {
                activity?.AddEvent(new("update existing user"));

                await _addUserToStore(existingUser.Value with { GivenName = Name.Create(givenName) });

                return Result.Success();
            }

            activity?.AddEvent(new("create new user"));

            var rootFolderId = Guid.NewGuid();
            var parentFolderId = Guid.Empty;
            await _addUserToStore(new FotoUser(userId, Name.Create(givenName), rootFolderId));

            await _addFolderToStore(new Folder(rootFolderId, parentFolderId, Name.Create("Root")));

            return Result.Success();
        }
    }
}