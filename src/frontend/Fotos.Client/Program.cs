using FluentValidation;
using Fotos.Client.Adapters;
using Fotos.Client.Api.Adapters;
using Fotos.Client.Api.PhotoAlbums;
using Fotos.Client.Api.PhotoFolders;
using Fotos.Client.Api.Photos;
using Fotos.Client.Components;
using Fotos.Client.Features.PhotoFolders;
using Fotos.Client.Hubs;
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
builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
builder.Services.AddProblemDetails();
builder.Services.AddMemoryCache();

// Business
builder.Services.AddPhotosBusiness();

// Adapters
builder.Services.AddPhotosAdapters(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(options => options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]));
builder.Services.AddFotosApi(builder.Configuration);
builder.Services.AddTransient<RealTimeMessageService>();

// Blazor features
builder.Services.AddSingleton<List<SessionData>>(_ => []);
builder.Services.AddScoped<SessionDataStorage>();
builder.Services.AddScoped<CustomCircuitHandler>();
builder.Services.AddScoped<CircuitHandler>(sp => sp.GetRequiredService<CustomCircuitHandler>());
builder.Services.AddScoped<SessionData>(sp => sp.GetRequiredService<CustomCircuitHandler>().SessionData);

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

app.MapPhotoFolderEndpoints();
app.MapPhotoAlbumEndpoints();
app.MapPhotosEndpoints();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<PhotosHub>("/photoshub");

await app.RunAsync();

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public partial class Program
{
    protected Program() { }
}