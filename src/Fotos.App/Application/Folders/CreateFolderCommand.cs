using CSharpFunctionalExtensions;
using Fotos.App.Domain;

namespace Fotos.App.Application.Folders;

internal sealed record CreateFolderCommand(Guid ParentFolderId, string Name) : ICommand
{
    internal sealed class Handler : ICommandHandler<CreateFolderCommand>
    {
        private readonly AddFolderToStore _addFolderToStore;
        public Handler(AddFolderToStore addFolderToStore)
        {
            _addFolderToStore = addFolderToStore;
        }
        public async Task<Result> Handle(CreateFolderCommand command, CancellationToken cancellationToken)
        {
            var folder = Folder.Create(Guid.NewGuid(), command.ParentFolderId, command.Name);

            await _addFolderToStore(folder);

            return Result.Success();
        }
    }
}
