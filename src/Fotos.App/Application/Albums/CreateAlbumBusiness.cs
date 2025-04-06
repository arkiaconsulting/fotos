using Fotos.App.Domain;

namespace Fotos.App.Application.Albums;

internal sealed class CreateAlbumBusiness
{
    private readonly AddAlbumToStore _addAlbumToStore;

    public CreateAlbumBusiness(AddAlbumToStore addAlbumToStore)
    {
        _addAlbumToStore = addAlbumToStore;
    }

    public async Task Process(Guid folderId, string name)
    {
        var album = Album.Create(Guid.NewGuid(), folderId, name);

        await _addAlbumToStore(album);
    }
}
