using Fotos.App.Domain;

namespace Fotos.App.Application.Photos;

internal sealed record RenamePhotoCommand(PhotoId Id, string Title) : ICommand
{
    internal sealed class Handler : ICommandHandler<RenamePhotoCommand>
    {
        private readonly AddPhotoToStore _storePhotoData;
        private readonly GetPhotoFromStore _getPhoto;

        public Handler(
            AddPhotoToStore storePhotoData,
            GetPhotoFromStore getPhoto)
        {
            _storePhotoData = storePhotoData;
            _getPhoto = getPhoto;
        }

        public async Task<Result> Handle(RenamePhotoCommand command, CancellationToken cancellationToken)
        {
            var photo = (await _getPhoto(command.Id)).WithTitle(command.Title);

            await _storePhotoData(photo);

            return Result.Success();
        }
    }
}
