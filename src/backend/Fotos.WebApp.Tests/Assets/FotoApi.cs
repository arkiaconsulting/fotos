using Fotos.WebApp.Features.Photos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.WebApp.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed class FotoApi : WebApplicationFactory<Program>
{
    internal List<Photo> Photos => Services.GetRequiredService<List<Photo>>();

    public FotoApi() => ClientOptions.BaseAddress = new Uri("https://localhost");
}
