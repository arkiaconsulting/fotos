using Fotos.App.Domain;

namespace Fotos.App.Application.Albums;

internal readonly record struct AlbumAugmented(Album Album, int PhotoCount);