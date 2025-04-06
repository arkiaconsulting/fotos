using Fotos.App.Features.PhotoFolders;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.Tests.Frontend.Assets.InMemory.Api;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddInMemoryPhotosApi(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryPhotosApi>();
        services.AddSingleton<AddPhoto>(sp => sp.GetRequiredService<InMemoryPhotosApi>().Add);
        services.AddSingleton<ListPhotos>(sp => sp.GetRequiredService<InMemoryPhotosApi>().List);
        services.AddSingleton<RemovePhoto>(sp => sp.GetRequiredService<InMemoryPhotosApi>().Remove);
        services.AddSingleton<GetPhoto>(sp => sp.GetRequiredService<InMemoryPhotosApi>().Get);
        services.AddSingleton<UpdatePhoto>(sp => sp.GetRequiredService<InMemoryPhotosApi>().Update);
        services.AddSingleton<GetOriginalUri>(sp => sp.GetRequiredService<InMemoryPhotosApi>().GetOriginalUri);
        services.AddSingleton<GetThumbnailUri>(sp => sp.GetRequiredService<InMemoryPhotosApi>().GetThumbnailUri);

        return services;
    }
}
