namespace Fotos.Application.Albums;

public readonly record struct AlbumAugmented(Album Album, int PhotoCount);