﻿namespace Fotos.Client.Api.PhotoAlbums;

internal readonly record struct AlbumDto(Guid Id, Guid FolderId, string Name);