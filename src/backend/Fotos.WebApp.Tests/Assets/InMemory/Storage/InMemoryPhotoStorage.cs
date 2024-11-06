using Fotos.Client.Features.Photos;
using System.Diagnostics.CodeAnalysis;

namespace Fotos.WebApp.Tests.Assets.InMemory.Storage;

[SuppressMessage("Design", "CA1812", Justification = "Instantiated by DI")]
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
internal sealed class InMemoryPhotoStorage
{
    public Task Add(PhotoId _, Stream _1, string _2) => Task.CompletedTask;

    public Task<Uri> GetOriginalStorageUri(PhotoId _) => Task.FromResult(new Uri("https://localhost"));

    public Task<Uri> GetThumbnailStorageUri(PhotoId _) => Task.FromResult(new Uri("https://localhost"));
}
