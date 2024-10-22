using Fotos.Client.Components;
using Fotos.Client.Features.PhotoFolders;
using Fotos.Client.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureWebJobs(builder => builder.AddServiceBus());

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(options => options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]));
builder.Services.AddFotosApi();
builder.Services.AddTransient<RealTimeMessageService>();

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<PhotosHub>("/photoshub");

app.Run();
