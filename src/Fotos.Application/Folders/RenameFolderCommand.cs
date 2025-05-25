namespace Fotos.Application.Folders;

public record RenameFolderCommand(Guid ParentFolderId, Guid FolderId, string Name) : ICommand
{
    internal sealed class Handler : ICommandHandler<RenameFolderCommand>
    {
        private readonly UpdateFolderInStore _updateFolder;
        public Handler(UpdateFolderInStore updateFolder)
        {
            _updateFolder = updateFolder;
        }

        async Task<Result> ICommandHandler<RenameFolderCommand>.Handle(RenameFolderCommand command, CancellationToken cancellationToken)
        {
            await _updateFolder(command.ParentFolderId, command.FolderId, Core.Name.Create(command.Name));

            return Result.Success();
        }
    }
}
