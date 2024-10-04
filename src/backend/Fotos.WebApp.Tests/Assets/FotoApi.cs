using Microsoft.AspNetCore.Mvc.Testing;

namespace Fotos.WebApp.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed class FotoApi : WebApplicationFactory<Program>
{
    public FotoApi() => ClientOptions.BaseAddress = new Uri("https://localhost");
}
