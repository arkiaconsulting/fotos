using Bunit.TestDoubles;
using Fotos.App.Adapters.RealTimeMessaging;
using Fotos.App.Application.Albums;
using Fotos.App.Application.Folders;
using Fotos.App.Application.Photos;
using Fotos.App.Application.User;
using Fotos.App.Domain;
using Fotos.Tests.Assets.InMemory.DataStore;
using Fotos.Tests.Assets.InMemory.Messaging;
using Fotos.Tests.Assets.InMemory.Storage;
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

    internal List<Folder> Folders { get; } = [];
    internal List<Album> Albums { get; } = [];
    internal List<Photo> Photos { get; } = [];
    internal List<FotoUser> Users { get; } = [];

    public Guid RootFolderId { get; } = Guid.NewGuid();

    public TestAuthorizationContext AuthContext { get; }

    public FotosTestContext()
    {
        AuthContext = this.AddTestAuthorization()
            .SetClaims([new(ClaimTypes.NameIdentifier, Tests.Assets.Authentication.Constants.TestUserId)])
            .SetAuthenticationType(Tests.Assets.Authentication.Constants.TestProvider);

        ConfigureServices();
        SetupMudProviders();
    }

    private void ConfigureServices()
    {
        Services.AddAccountBusiness()
            .AddFolderBusiness()
            .AddAlbumBusiness()
            .AddPhotoBusiness();

        Folders.Add(Folder.Create(RootFolderId, Guid.Empty, "Root"));
        Services.AddInMemoryFolderDataStore(Folders);
        Services.AddInMemoryUserDataStore(Users);
        Services.AddInMemoryAlbumDataStore(Albums);
        Services.AddInMemoryPhotoDataStore(Photos);
        Services.AddInMemoryPhotoStorage();
        Services.AddInMemoryMessagePublishers();

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
        Users.Add(new FotoUser(FotoUserId.Create(Tests.Assets.Authentication.Constants.TestProvider, Tests.Assets.Authentication.Constants.TestUserId), Name.Create(givenName), RootFolderId));
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
