namespace Fotos.App.Application.Folders;

internal record RenameFolderCommand(Guid ParentFolderId, Guid FolderId, string Name) : ICommand
{
    internal sealed class Handler : ICommandHandler<RenameFolderCommand>
    {
        private readonly UpdateFolderInStore _updateFolder;
        public Handler(UpdateFolderInStore updateFolder)
        {
            _updateFolder = updateFolder;
        }

        public async Task<Result> Handle(RenameFolderCommand command, CancellationToken cancellationToken)
        {
            await _updateFolder(command.ParentFolderId, command.FolderId, Domain.Name.Create(command.Name));

            return Result.Success();
        }
    }
}
