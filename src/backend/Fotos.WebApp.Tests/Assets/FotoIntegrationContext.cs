using Azure.Storage.Blobs;
using Fotos.WebApp.Features.Photos;
using Fotos.WebApp.Features.Photos.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fotos.WebApp.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed class FotoIntegrationContext
{
    internal AddPhotoToMainStorage AddPhotoToMainStorage => _host.Services.GetRequiredService<AddPhotoToMainStorage>();
    internal BlobContainerClient PhotosContainer
    {
        get
        {
            var storage = _host.Services.GetRequiredService<BlobServiceClient>();
            var containerName = _host.Services.GetRequiredService<IConfiguration>()["MainStorage:PhotosContainer"];

            return storage.GetBlobContainerClient(containerName);
        }
    }

    private readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureServices(ConfigureServices)
        .ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MainStorage:blobServiceUri"] = "UseDevelopmentStorage=true",
                ["MainStorage:PhotosContainer"] = "fotostests",
            });
        })
        .Build();

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddFotosAzureStorage(context.Configuration);
    }
}
