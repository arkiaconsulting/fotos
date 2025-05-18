using Azure.Storage.Blobs;
using Fotos.App.Application.Photos;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;

namespace Fotos.App.Adapters.Storage;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddAzureStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AzurePhotoStorage>()
        .AddScoped<AddPhotoToMainStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().AddOriginalPhoto)
        .AddScoped<GetOriginalStorageUri>(sp => sp.GetRequiredService<AzurePhotoStorage>().GetOriginalUri)
        .AddScoped<GetThumbnailStorageUri>(sp => sp.GetRequiredService<AzurePhotoStorage>().GetThumbnailUri)
        .AddScoped<ReadOriginalPhotoFromStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().ReadOriginalPhoto)
        .AddScoped<AddPhotoToThumbnailStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().AddPhotoToThumbnailStorage)
        .AddScoped<RemovePhotoOriginalFromStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().RemovePhotoOriginal)
        .AddScoped<RemovePhotoThumbnailFromStorage>(sp => sp.GetRequiredService<AzurePhotoStorage>().RemovePhotoThumbnail);

        services.AddOptions<StorageOptions>()
            .BindConfiguration(StorageOptions.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Services
            .AddAzureClients(builder =>
        {
            builder.AddClient<BlobServiceClient, BlobClientOptions>((clientOptions, credential, provider) =>
            {
                var options = provider.GetRequiredService<IOptions<StorageOptions>>().Value;

                return new BlobServiceClient(options.BlobServiceUri, credential, clientOptions);
            });

            builder.AddClient<BlobContainerClient, BlobClientOptions>(
                (_, _, sp) =>
                {
                    var options = sp.GetRequiredService<IOptions<StorageOptions>>().Value;

                    return sp.GetRequiredService<BlobServiceClient>().GetBlobContainerClient(options.PhotosContainer);

                }).WithName(Constants.PhotosBlobContainer);
        });

        services.AddSingleton<SasUriGenerator>();

        return services;
    }
}
