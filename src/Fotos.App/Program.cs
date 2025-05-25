using Azure.Core;
using Azure.Identity;
using Fotos.Adapters.DataStore;
using Fotos.App.Adapters.Blazor;
using Fotos.App.Adapters.Imaging;
using Fotos.App.Adapters.Messaging;
using Fotos.App.Adapters.RealTimeMessaging;
using Fotos.App.Adapters.Storage;
using Fotos.App.Api.Account;
using Fotos.App.Application.User;
using Fotos.App.Authentication;
using Fotos.App.Observability;
using Fotos.Application;
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
builder.Services.AddRazorComponents(options =>
    options.DetailedErrors = builder.Configuration.GetValue("DetailedErrors", false)).AddInteractiveServerComponents();
builder.Services.AddMudServices(options =>
{
    options.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    options.PopoverOptions.ThrowOnDuplicateProvider = false;
});
builder.Services.AddMemoryCache();

// Authentication
builder.Services.AddFotosAuthentication(builder.Configuration);

// Business
builder.Services.AddApplication();

builder.Services.AddScoped<SessionDataStorage>();

// Adapters
builder.Services.AddCosmos();
builder.Services.AddAzureStorage();
builder.Services.AddServiceBus();
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