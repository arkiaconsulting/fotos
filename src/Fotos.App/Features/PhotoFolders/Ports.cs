﻿using Fotos.App.Api.Photos;
using Fotos.App.Features.PhotoAlbums;
using Fotos.App.Features.Photos;

namespace Fotos.App.Features.PhotoFolders;

internal delegate Task<IReadOnlyCollection<PhotoDto>> ListPhotos(AlbumId albumId);
internal delegate Task<Guid> AddPhoto(AlbumId albumId, PhotoToUpload Binary);
internal delegate Task RemovePhoto(PhotoId photoId);
internal delegate Task<Uri> GetOriginalUri(PhotoId photoId);
internal delegate Task<Uri> GetThumbnailUri(PhotoId photoId);
internal delegate Task UpdatePhoto(PhotoId photoId, string title);
internal delegate Task<PhotoDto> GetPhoto(PhotoId photoId);