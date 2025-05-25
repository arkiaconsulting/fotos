namespace Fotos.Application.Photos;

public sealed record RenamePhotoCommand(PhotoId Id, string Title) : ICommand
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

        async Task<Result> ICommandHandler<RenamePhotoCommand>.Handle(RenamePhotoCommand command, CancellationToken cancellationToken)
        {
            var photo = (await _getPhoto(command.Id)).WithTitle(command.Title);

            await _storePhotoData(photo);

            return Result.Success();
        }
    }
}
