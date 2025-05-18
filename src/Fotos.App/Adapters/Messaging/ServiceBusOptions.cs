using System.ComponentModel.DataAnnotations;

namespace Fotos.App.Adapters.Messaging;

internal sealed class ServiceBusOptions
{
    public const string Section = "ServiceBus";

    [Required]
    public string FullyQualifiedNamespace { get; set; } = default!;

    [Required]
    public string MainTopic { get; set; } = default!;
}
