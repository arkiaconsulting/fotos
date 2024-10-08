using Fotos.Client.Features.PhotoFolders;
using Fotos.Client.Features.Profile;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.Client.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1812", Justification = "<Pending>")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed class FotosTestContext : TestContext
{
    internal List<Folder> Folders => Services.GetRequiredService<List<Folder>>();

    public Guid RootFolderId { get; } = Guid.NewGuid();

    public FotosTestContext() => ConfigureServices();

    private void ConfigureServices()
    {
        Services.AddSingleton<List<Folder>>();
        Services.AddSingleton<ListFolders>(sp =>
        {
            var folders = sp.GetRequiredService<List<Folder>>();

            return (Guid guid) => Task.FromResult<IReadOnlyCollection<Folder>>(folders.Where(f => f.ParentId == guid).ToList());
        });
        Services.AddSingleton<GetRootFolderId>(sp => () => Task.FromResult(RootFolderId));
    }
}
