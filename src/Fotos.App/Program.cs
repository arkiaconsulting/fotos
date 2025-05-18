using Azure.Core;
using Azure.Identity;
using Fotos.App;
using Fotos.App.Adapters.Blazor;
using Fotos.App.Adapters.DataStore;
using Fotos.App.Adapters.Imaging;
using Fotos.App.Adapters.Messaging;
using Fotos.App.Adapters.RealTimeMessaging;
using Fotos.App.Adapters.Storage;
using Fotos.App.Api.Account;
using Fotos.App.Application.Albums;
using Fotos.App.Application.Folders;
using Fotos.App.Application.Photos;
using Fotos.App.Application.User;
using Fotos.App.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

TokenCredential credential = builder.Environment.IsProduction()
    ? new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(Environment.GetEnvironmentVariable("AZURE_CLIENT_ID")))
    : new DefaultAzureCredential();
builder.Services.AddSingleton(_ => credential);

builder.Host.ConfigureWebJobs(builder => builder.AddServiceBus());

builder.Configuration.AddAzureAppConfiguration(config =>
{
    var endpoint = new Uri(Environment.GetEnvironmentVariable("APP_CONFIG_ENDPOINT")
    ?? "http://notset");
    config.Connect(endpoint, credential)
    .ConfigureKeyVault(options => options.SetCredential(credential))
    .Select(KeyFilter.Any, "common")
    .Select(KeyFilter.Any, "fotos");
}, false);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices(options =>
{
    options.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    options.PopoverOptions.ThrowOnDuplicateProvider = false;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("fotos", new() { Title = "Fotos" });
    options.IncludeXmlComments(typeof(Program).Assembly.Location.Replace("dll", "xml", StringComparison.OrdinalIgnoreCase));
    options.NonNullableReferenceTypesAsRequired();
});
builder.Services.AddMemoryCache();

// Authentication
builder.Services.AddFotosAuthentication(builder.Configuration);

// Business
builder.Services.AddFolderBusiness();
builder.Services.AddPhotoBusiness();
builder.Services.AddAlbumBusiness();
builder.Services.AddAccountBusiness();
builder.Services.AddScoped<SessionDataStorage>();

// Adapters
builder.Services.AddCosmos(builder.Configuration);
builder.Services.AddAzureStorage(builder.Configuration);
builder.Services.AddServiceBus(builder.Configuration);
builder.Services.AddImageProcessing();
builder.Services.AddSignalRFotosHub();
builder.Services.AddCustomCircuitHandler();

// Instrumentation
builder.AddFotosInstrumentation();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor
        | ForwardedHeaders.XForwardedProto
        | ForwardedHeaders.XForwardedHost;

});

var app = builder.Build();

app.UseResponseCompression();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseSwagger(options => options.RouteTemplate = "{documentName}/openapi.json");
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/fotos/openapi.json", "Fotos Client Api"));
}

app.MapAccountEndpoints();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<Fotos.App.Components.App>()
    .AddInteractiveServerRenderMode();

app.MapHub<PhotosHub>("/photoshub");

await app.RunAsync();

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public partial class Program
{
    protected Program() { }
}