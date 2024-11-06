﻿using Fotos.Client.Api.Photos;
using Fotos.Client.Features.Photos;
using Fotos.WebApp.Tests.Assets.Authentication;
using Fotos.WebApp.Tests.Assets.InMemory.DataStore;
using Fotos.WebApp.Tests.Assets.InMemory.Messaging;
using Fotos.WebApp.Tests.Assets.InMemory.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.WebApp.Tests.Assets;

public sealed class FotoApi : WebApplicationFactory<Program>
{
    internal List<Photo> Photos => Services.GetRequiredService<List<Photo>>();
    internal List<PhotoId> PhotoRemovedMessageSink => Services.GetRequiredKeyedService<List<PhotoId>>("messages-removed");
    internal List<PhotoId> PhotoUploadedMessageSink => Services.GetRequiredKeyedService<List<PhotoId>>("messages-uploaded");

    public FotoApi()
    {
        ClientOptions.BaseAddress = new Uri("https://localhost");
        ClientOptions.AllowAutoRedirect = false;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureServices(services =>
        {
            services.AddInMemoryPhotoStorage()
            .AddInMemoryPhotoDataStore()
            .AddInMemoryAlbumDataStore()
            .AddInMemoryFolderDataStore()
            .AddInMemoryUserDataStore()
            .AddInMemoryMessagePublishers();

            services.AddFakeAuthentication();
        });

        builder.ConfigureAppConfiguration(ConfigureAppConfiguration);
    }

    private static void ConfigureAppConfiguration(WebHostBuilderContext _, IConfigurationBuilder builder) =>
        builder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AzureWebJobs.OnShouldProduceThumbnail.Disabled"] = "true",
            ["AzureWebJobs.OnShouldRemovePhotoBinaries.Disabled"] = "true",
            ["AzureWebJobs.OnShouldExtractExifMetadata.Disabled"] = "true",
            ["Instrumentation:ServiceName"] = "tests-fotos-app",
        });

    public override async ValueTask DisposeAsync()
    {
        // Due to pipelines runs transient failures
        try
        {
            await base.DisposeAsync();
        }
        catch (ObjectDisposedException)
        {
            // OK
        }
    }
}
