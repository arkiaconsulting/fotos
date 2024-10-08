﻿namespace Fotos.Client.Features.PhotoFolders;

internal delegate Task<IReadOnlyCollection<Folder>> ListFolders(Guid parentId);
internal delegate Task CreateFolder(Guid parentId, string folderName);
internal delegate Task<Folder> GetFolder(Guid folderId);
internal delegate Task RemoveFolder(Guid folderId);
internal delegate Task CreateAlbum(Guid folderId, string albumName);
internal delegate Task<IReadOnlyCollection<Album>> ListAlbums(Guid folderId);
