using Fotos.App.Domain;

namespace Fotos.App.Application.Photos;

internal sealed record AddPhotoCommand(Guid FolderId, Guid AlbumId, Stream Photo, string ContentType, string FileName)
    : ICommand<Guid>
{
    internal sealed class Handler : ICommandHandler<AddPhotoCommand, Guid>
    {
        private readonly AddPhotoToStore _storePhotoData;
        private readonly AddPhotoToMainStorage _addPhotoToMainStorage;
        private readonly OnNewPhotoUploaded _onNewPhotoUploaded;

        public Handler(
            AddPhotoToStore storePhotoData,
            AddPhotoToMainStorage addPhotoToMainStorage,
            OnNewPhotoUploaded onNewPhotoUploaded)
        {
            _storePhotoData = storePhotoData;
            _addPhotoToMainStorage = addPhotoToMainStorage;
            _onNewPhotoUploaded = onNewPhotoUploaded;
        }

        public async Task<Result<Guid>> Handle(AddPhotoCommand command, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            var photoId = new PhotoId(command.FolderId, command.AlbumId, id);

            await _addPhotoToMainStorage(photoId, command.Photo, command.ContentType);

            var photoData = new Photo(photoId, command.FileName);

            await _storePhotoData(photoData);

            await _onNewPhotoUploaded(photoId);

            return id;
        }
    }
}