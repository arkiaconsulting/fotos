using Fotos.WebApp.Features.Photos;
using Fotos.WebApp.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Fotos.WebApp.Tests.Assets;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public sealed class FotoApi : WebApplicationFactory<Program>
{
    internal List<PhotoEntity> Photos => Services.GetRequiredService<List<PhotoEntity>>();

    public FotoApi() => ClientOptions.BaseAddress = new Uri("https://localhost");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureServices(services =>
            services.AddScoped<AddPhotoToMainStorage>(_ => (_, _) => Task.CompletedTask)
            .AddScoped<OnNewPhotoUploaded>((_) => (_) => Task.CompletedTask)
        );
    }
}
