using CSharpFunctionalExtensions;
using Fotos.App.Domain;

namespace Fotos.App.Application.Albums;

internal sealed record CreateAlbumCommand(Guid FolderId, string Name) : ICommand
{
    internal sealed class Handler : ICommandHandler<CreateAlbumCommand>
    {
        private readonly AddAlbumToStore _addAlbum;

        public Handler(AddAlbumToStore addAlbum)
        {
            _addAlbum = addAlbum;
        }

        public async Task<Result> Handle(CreateAlbumCommand command, CancellationToken cancellationToken)
        {
            var album = Album.Create(Guid.NewGuid(), command.FolderId, command.Name);

            await _addAlbum(album);

            return Result.Success();
        }
    }
}
