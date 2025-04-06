using Bunit.TestDoubles;
using Fotos.App;
using Fotos.App.Adapters;
using Fotos.App.Api.PhotoAlbums;
using Fotos.App.Api.Photos;
using Fotos.App.Api.Shared;
using Fotos.App.Api.Types;
using Fotos.App.Application.Albums;
using Fotos.App.Application.Folders;
using Fotos.App.Application.User;
using Fotos.App.Hubs;
using Fotos.Tests.Backend.Assets.Authentication;
using Fotos.Tests.Backend.Assets.InMemory.DataStore;
using Fotos.Tests.Frontend.Assets.InMemory.Api;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System.Security.Claims;

namespace Fotos.Tests.Frontend.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
internal sealed class FotosTestContext : TestContext
{
    internal IRenderedFragment SnackBar => _snackBarComponent!;
    internal IRenderedFragment Popover => _popoverProvider!;
    internal IRenderedFragment Dialog => _dialogProvider!;
    internal NavigationManager NavigationManager => Services.GetRequiredService<NavigationManager>();

    private IRenderedComponent<MudSnackbarProvider>? _snackBarComponent;
    private IRenderedComponent<MudPopoverProvider>? _popoverProvider;
    private IRenderedComponent<MudDialogProvider>? _dialogProvider;

    internal List<Folder> Folders => Services.GetRequiredService<List<Folder>>();
    internal List<Album> Albums { get; } = [];
    internal List<Photo> Photos => Services.GetRequiredService<List<Photo>>();
    internal List<FotoUser> Users { get; } = [];

    public Guid RootFolderId { get; } = Guid.NewGuid();

    public TestAuthorizationContext AuthContext { get; }

    public FotosTestContext()
    {
        AuthContext = this.AddTestAuthorization()
            .SetClaims([new(ClaimTypes.NameIdentifier, Constants.TestUserId)])
            .SetAuthenticationType(Constants.TestProvider);

        ConfigureServices();
        SetupMudProviders();
    }

    private void ConfigureServices()
    {
        Services.SetRootFolderId(RootFolderId);
        Services.AddSingleton<InstrumentationConfig>();

        Services.AddAccountBusiness()
            .AddFolderBusiness()
            .AddAlbumBusiness();

        Services.AddInMemoryFolderDataStore();
        Services.AddInMemoryUserDataStore(Users);
        Services.AddInMemoryAlbumDataStore(Albums);

        Services.AddInMemoryPhotoDataStore()
            .AddInMemoryPhotosApi();

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

    public void AddUser(string givenName)
    {
        Users.Add(new FotoUser(FotoUserId.Create(Constants.TestProvider, Constants.TestUserId), Name.Create(givenName), RootFolderId));
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
