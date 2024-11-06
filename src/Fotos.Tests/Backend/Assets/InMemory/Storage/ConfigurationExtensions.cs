using Fotos.App.Api.Photos;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.Tests.Backend.Assets.InMemory.Storage;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddInMemoryPhotoStorage(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryPhotoStorage>();
        services.AddSingleton<AddPhotoToMainStorage>(sp => sp.GetRequiredService<InMemoryPhotoStorage>().Add);
        services.AddSingleton<GetOriginalStorageUri>(sp => sp.GetRequiredService<InMemoryPhotoStorage>().GetOriginalStorageUri);
        services.AddSingleton<GetThumbnailStorageUri>(sp => sp.GetRequiredService<InMemoryPhotoStorage>().GetThumbnailStorageUri);

        return services;
    }
}
