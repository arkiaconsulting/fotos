using Fotos.Application.Folders;

namespace Fotos.Application.User;

public sealed record AddUserCommand(FotoUserId UserId, string GivenName) : ICommand
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

        async Task<Result> ICommandHandler<AddUserCommand>.Handle(AddUserCommand command, CancellationToken cancellationToken)
        {
            var (userId, givenName) = command;
            var existingUser = await _findUserInStore(userId);

            if (existingUser is not null)
            {
                await _addUserToStore(existingUser.Value with { GivenName = Name.Create(givenName) });

                return Result.Success();
            }

            var rootFolderId = Guid.NewGuid();
            var parentFolderId = Guid.Empty;
            await _addUserToStore(new FotoUser(userId, Name.Create(givenName), rootFolderId));

            await _addFolderToStore(new Folder(rootFolderId, parentFolderId, Name.Create("Root")));

            return Result.Success();
        }
    }
}