using Bunit.TestDoubles;
using Fotos.App.Adapters;
using Fotos.App.Api.PhotoAlbums;
using Fotos.App.Api.PhotoFolders;
using Fotos.App.Api.Photos;
using Fotos.App.Hubs;
using Fotos.Tests.Backend.Assets.InMemory.DataStore;
using Fotos.Tests.Frontend.Assets.InMemory.Api;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;

namespace Fotos.Tests.Frontend.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
public sealed class FotosTestContext : TestContext
{
    internal IRenderedFragment SnackBar => _snackBarComponent!;
    internal IRenderedFragment Popover => _popoverProvider!;
    internal IRenderedFragment Dialog => _dialogProvider!;
    internal NavigationManager NavigationManager => Services.GetRequiredService<NavigationManager>();

    private IRenderedComponent<MudSnackbarProvider>? _snackBarComponent;
    private IRenderedComponent<MudPopoverProvider>? _popoverProvider;
    private IRenderedComponent<MudDialogProvider>? _dialogProvider;

    internal List<Folder> Folders => Services.GetRequiredService<List<Folder>>();
    internal List<Album> Albums => Services.GetRequiredService<List<Album>>();
    internal List<Photo> Photos => Services.GetRequiredService<List<Photo>>();

    public Guid RootFolderId { get; } = Guid.NewGuid();

    public TestAuthorizationContext AuthContext { get; }

    public FotosTestContext()
    {
        AuthContext = this.AddTestAuthorization();

        ConfigureServices();
        SetupMudProviders();
    }

    private void ConfigureServices()
    {
        Services.SetRootFolderId(RootFolderId);

        Services.AddInMemoryFolderDataStore()
            .AddInMemoryFoldersApi();

        Services.AddInMemoryAlbumDataStore()
            .AddInMemoryAlbumsApi();

        Services.AddInMemoryPhotoDataStore()
            .AddInMemoryPhotosApi();

        Services.AddInMemoryUserDataStore()
            .AddInMemoryUsersApi();

        Services.AddTransient<RealTimeMessageService, RealTimeServiceFake>();
        Services.AddScoped<SessionDataStorage, LocalStorageServiceFake>();
        Services.AddSingleton<SessionData>(_ => new([]));
    }

    private void SetupMudProviders()
    {
        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose;
        _snackBarComponent = RenderComponent<MudSnackbarProvider>();
        _popoverProvider = RenderComponent<MudPopoverProvider>();
        _dialogProvider = RenderComponent<MudDialogProvider>();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _snackBarComponent?.Dispose();
            _popoverProvider?.Dispose();
            _dialogProvider?.Dispose();
        }

        base.Dispose(disposing);
    }
}
