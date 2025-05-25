namespace Fotos.Application.Albums;

public sealed record CreateAlbumCommand(Guid FolderId, string Name) : ICommand
{
    internal sealed class Handler : ICommandHandler<CreateAlbumCommand>
    {
        private readonly AddAlbumToStore _addAlbum;

        public Handler(AddAlbumToStore addAlbum)
        {
            _addAlbum = addAlbum;
        }

        async Task<Result> ICommandHandler<CreateAlbumCommand>.Handle(CreateAlbumCommand command, CancellationToken cancellationToken)
        {
            var album = Album.Create(Guid.NewGuid(), command.FolderId, command.Name);

            await _addAlbum(album);

            return Result.Success();
        }
    }
}
