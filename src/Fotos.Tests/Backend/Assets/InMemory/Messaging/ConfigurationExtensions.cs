﻿using Fotos.App.Api.Photos;
using Fotos.App.Features.Photos;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.Tests.Backend.Assets.InMemory.Messaging;

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
