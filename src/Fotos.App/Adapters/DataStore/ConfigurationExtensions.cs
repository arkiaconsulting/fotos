using Fotos.App.Application.Folders;
using Fotos.App.Application.Photos;
using Fotos.App.Application.User;
using Fotos.App.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;

namespace Fotos.App.Adapters.DataStore;

internal static class ConfigurationExtensions
{
    public static IServiceCollection AddCosmos(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AzureCosmosDb>()
        .AddScoped<AddPhotoToStore>(sp => sp.GetRequiredService<AzureCosmosDb>().SavePhoto)
        .AddScoped<ListPhotosFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().ListPhotos)
        .AddScoped<RemovePhotoFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().RemovePhoto)
        .AddScoped<GetPhotoFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().GetPhoto)
        .AddScoped<AddFolderToStore>(sp => sp.GetRequiredService<AzureCosmosDb>().StoreFolder)
        .AddScoped<GetFoldersFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().GetFolders)
        .AddScoped<GetFolderFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().GetFolder)
        .AddScoped<RemoveFolderFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().RemoveFolder)
        .AddScoped<UpdateFolderInStore>(sp => sp.GetRequiredService<AzureCosmosDb>().UpsertFolder)
        .AddScoped<GetFolderAlbumsFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().GetAlbums)
        .AddScoped<AddAlbumToStore>(sp => sp.GetRequiredService<AzureCosmosDb>().StoreAlbum)
        .AddScoped<GetAlbumFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().GetAlbum)
        .AddScoped<AddSessionDataToStore>(sp => sp.GetRequiredService<AzureCosmosDb>().StoreSessionData)
        .AddScoped<GetSessionDataFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().GetSessionData)
        .AddScoped<FindUserInStore>(sp => sp.GetRequiredService<AzureCosmosDb>().FindUser)
        .AddScoped<AddUserToStore>(sp => sp.GetRequiredService<AzureCosmosDb>().StoreUser)
        .AddScoped<GetAlbumPhotoCountFromStore>(sp => sp.GetRequiredService<AzureCosmosDb>().GetAlbumPhotoCount);

        services.AddOptions<CosmosOptions>()
            .BindConfiguration(CosmosOptions.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddAzureClients(builder =>
        {
            builder.AddClient<Database, CosmosClientOptions>((_, credential, provider) =>
            {
                var options = provider.GetRequiredService<IOptions<CosmosOptions>>().Value;

                var clientBuilder = options.IsEmulator
                    ? new CosmosClientBuilder(options.Endpoint, options.AccountKey)
                    : new CosmosClientBuilder(options.Endpoint, credential);

                return clientBuilder
                   .WithContentResponseOnWrite(false)
                   .WithConnectionModeDirect()
                   .WithLimitToEndpoint(true)
                   .WithSystemTextJsonSerializerOptions(new(System.Text.Json.JsonSerializerDefaults.Web))
                   .Build()
                   .GetDatabase(options.DatabaseId);
            });

            builder.AddClient<Container, CosmosClientOptions>((_, _, provider) =>
            {
                var containerName = provider.GetRequiredService<IOptions<CosmosOptions>>().Value.PhotosContainerId;
                var client = provider.GetRequiredService<Database>();

                return client.GetContainer(containerName);
            }).WithName(Constants.PhotosClientName);

            builder.AddClient<Container, CosmosClientOptions>((_, _, provider) =>
            {
                var containerName = provider.GetRequiredService<IOptions<CosmosOptions>>().Value.AlbumsContainerId;
                var client = provider.GetRequiredService<Database>();

                return client.GetContainer(containerName);
            }).WithName(Constants.AlbumsClientName);

            builder.AddClient<Container, CosmosClientOptions>((_, _, provider) =>
            {
                var containerName = provider.GetRequiredService<IOptions<CosmosOptions>>().Value.FoldersContainerId;
                var client = provider.GetRequiredService<Database>();

                return client.GetContainer(containerName);
            }).WithName(Constants.FoldersClientName);

            builder.AddClient<Container, CosmosClientOptions>((_, _, provider) =>
            {
                var containerName = provider.GetRequiredService<IOptions<CosmosOptions>>().Value.SessionDataContainerId;
                var client = provider.GetRequiredService<Database>();

                return client.GetContainer(containerName);
            }).WithName(Constants.SessionDataClientName);

            builder.AddClient<Container, CosmosClientOptions>((_, _, provider) =>
            {
                var containerName = provider.GetRequiredService<IOptions<CosmosOptions>>().Value.UsersContainerId;
                var client = provider.GetRequiredService<Database>();

                return client.GetContainer(containerName);
            }).WithName(Constants.UsersClientName);
        });

        return services;
    }

}
