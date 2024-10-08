using AutoFixture.Xunit2;
using Fotos.Client.Components.Pages;
using Fotos.Client.Features.PhotoFolders;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.Client.Tests;

[Trait("Category", "Unit")]
public sealed class UnitTest1 : IClassFixture<TestContext>
{
    private List<Folder> Folders => _testContext.Services.GetRequiredService<List<Folder>>();

    private readonly TestContext _testContext;
    private readonly Guid _rootFolderId = Guid.NewGuid();

    public UnitTest1(TestContext testContext)
    {
        _testContext = testContext;
        _testContext.Services.AddSingleton<List<Folder>>();
        _testContext.Services.AddSingleton<ListFolders>(sp =>
        {
            var folders = sp.GetRequiredService<List<Folder>>();

            return (Guid guid) => Task.FromResult<IReadOnlyCollection<Folder>>(folders.Where(f => f.ParentId == guid).ToList());
        });
        _testContext.Services.AddSingleton<GetRootFolderId>(sp => () => Task.FromResult(_rootFolderId));
    }

    [Theory(DisplayName = "Home page should show folders at root"), AutoData]
    public void Test01(string folderName)
    {
        Folders.Add(new Folder { ParentId = _rootFolderId, Name = folderName });
        var home = _testContext.RenderComponent<Home>();

        home.Find("ul").InnerHtml.MarkupMatches($"<li>{folderName}</li>");
    }
}