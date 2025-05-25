namespace Fotos.Application.Photos;

public sealed record RemovePhotoCommand(PhotoId Id) : ICommand
{
    internal sealed class Handler : ICommandHandler<RemovePhotoCommand>
    {
        private readonly RemovePhotoFromStore _removePhotoFromStore;
        private readonly OnPhotoRemoved _onPhotoRemoved;

        public Handler(RemovePhotoFromStore removePhotoFromStore,
            OnPhotoRemoved onPhotoRemoved)
        {
            _removePhotoFromStore = removePhotoFromStore;
            _onPhotoRemoved = onPhotoRemoved;
        }

        async Task<Result> ICommandHandler<RemovePhotoCommand>.Handle(RemovePhotoCommand command, CancellationToken cancellationToken)
        {
            var photoId = command.Id;

            await _removePhotoFromStore(photoId);

            await _onPhotoRemoved(photoId);

            return Result.Success();
        }
    }
}
