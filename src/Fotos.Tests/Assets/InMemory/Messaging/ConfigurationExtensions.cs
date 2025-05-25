using Fotos.Application.Photos;
using Fotos.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.Tests.Assets.InMemory.Messaging;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddInMemoryMessagePublishers(this IServiceCollection services)
    {
        services.AddKeyedSingleton<List<PhotoId>>("messages-uploaded");
        services.AddKeyedSingleton<List<PhotoId>>("messages-removed");
        services.AddSingleton<InMemoryMessagePublisher>();
        services.AddSingleton<OnNewPhotoUploaded>(sp => sp.GetRequiredService<InMemoryMessagePublisher>().PublishUploaded);
        services.AddSingleton<OnPhotoRemoved>(sp => sp.GetRequiredService<InMemoryMessagePublisher>().PublishRemoved);

        return services;
    }
}
