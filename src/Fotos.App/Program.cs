using Fotos.App;
using Fotos.App.Adapters;
using Fotos.App.Api.Account;
using Fotos.App.Application.Albums;
using Fotos.App.Application.Folders;
using Fotos.App.Application.Photos;
using Fotos.App.Application.User;
using Fotos.App.Authentication;
using Fotos.App.Hubs;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.ResponseCompression;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureWebJobs(builder => builder.AddServiceBus());

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

// Adapters
builder.Services.AddPhotosAdapters(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(options => options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]));
builder.Services.AddTransient<RealTimeMessageService>();

// Blazor features
builder.Services.AddSingleton<List<SessionData>>(_ => []);
builder.Services.AddScoped<SessionDataStorage>();
builder.Services.AddScoped<CustomCircuitHandler>();
builder.Services.AddScoped<CircuitHandler>(sp => sp.GetRequiredService<CustomCircuitHandler>());
builder.Services.AddScoped<SessionData>(sp => sp.GetRequiredService<CustomCircuitHandler>().SessionData);

// Instrumentation
builder.AddFotosInstrumentation();

var app = builder.Build();

app.UseResponseCompression();

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

app.UseHttpsRedirection();

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