using FluentAssertions;
using Fotos.Tests.Backend.Assets;

namespace Fotos.Tests.Backend;

[Trait("Category", "Unit")]
public sealed class OpenApiTests : IClassFixture<FotoApi>
{
    private readonly FotoApi _fotoApi;

    public OpenApiTests(FotoApi fotoApi) => _fotoApi = fotoApi;

    [Fact]
    public async Task Test01()
    {
        var client = _fotoApi.CreateClient();
        using var response = await client.GetAsync(new Uri("fotos/openapi.json", UriKind.Relative));

        response.Should().Be200Ok();
        await File.WriteAllTextAsync(@"C:\temp\foto.json", await response.Content.ReadAsStringAsync());
    }
}
