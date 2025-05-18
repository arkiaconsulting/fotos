using System.ComponentModel.DataAnnotations;

namespace Fotos.App.Adapters.Storage;

internal sealed class StorageOptions
{
    public const string Section = "MainStorage";

    [Required]
    public Uri BlobServiceUri { get; set; } = default!;

    [Required]
    public string PhotosContainer { get; set; } = default!;
}
