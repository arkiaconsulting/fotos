namespace Fotos.Application.Folders;

public sealed record RemoveFolderCommand(Guid ParentFolderId, Guid FolderId) : ICommand
{
    internal sealed class Handler : ICommandHandler<RemoveFolderCommand>
    {
        private readonly RemoveFolderFromStore _removeFolderFromStore;
        public Handler(RemoveFolderFromStore removeFolderFromStore)
        {
            _removeFolderFromStore = removeFolderFromStore;
        }

        async Task<Result> ICommandHandler<RemoveFolderCommand>.Handle(RemoveFolderCommand command, CancellationToken cancellationToken)
        {
            await _removeFolderFromStore(command.ParentFolderId, command.FolderId);

            return Result.Success();
        }
    }
}
