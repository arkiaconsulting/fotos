using System.ComponentModel.DataAnnotations;

namespace Fotos.App.Adapters.DataStore;

internal sealed class CosmosOptions
{
    public const string Section = "Cosmos";

    [Required]
    public string Endpoint { get; set; } = default!;

    public string AccountKey { get; set; } = default!;

    [Required]
    public string DatabaseId { get; set; } = default!;

    [Required]
    public string PhotosContainerId { get; set; } = default!;

    [Required]
    public string AlbumsContainerId { get; set; } = default!;

    [Required]
    public string FoldersContainerId { get; set; } = default!;

    [Required]
    public string SessionDataContainerId { get; set; } = default!;

    [Required]
    public string UsersContainerId { get; set; } = default!;

    public bool IsEmulator => !string.IsNullOrWhiteSpace(AccountKey);
}