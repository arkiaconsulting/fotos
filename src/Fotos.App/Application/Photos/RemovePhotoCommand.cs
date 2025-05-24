using Fotos.App.Domain;

namespace Fotos.App.Application.Photos;

internal sealed record RemovePhotoCommand(PhotoId Id) : ICommand
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
        public async Task<Result> Handle(RemovePhotoCommand command, CancellationToken cancellationToken)
        {
            var photoId = command.Id;

            await _removePhotoFromStore(photoId);

            await _onPhotoRemoved(photoId);

            return Result.Success();
        }
    }
}
