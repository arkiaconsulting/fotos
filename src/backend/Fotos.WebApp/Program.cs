using FluentValidation;
using Fotos.WebApp.Features.PhotoAlbums;
using Fotos.WebApp.Features.PhotoFolders;
using Fotos.WebApp.Features.Photos;
using Fotos.WebApp.Features.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("fotos", new() { Title = "Fotos" });
    options.IncludeXmlComments(typeof(Program).Assembly.Location.Replace("dll", "xml", StringComparison.OrdinalIgnoreCase));
    options.NonNullableReferenceTypesAsRequired();
    options.MapType<Name>(() => new() { Type = "string" });
});
builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
builder.Services.AddProblemDetails();

// Business
builder.Services.AddPhotosBusiness();

// Adapters
builder.Services.AddPhotoFoldersAdapters();
builder.Services.AddPhotoAlbumAdapters();
builder.Services.AddPhotosAdapters();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => options.RouteTemplate = "{documentName}/openapi.json");
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/fotos/openapi.json", "Fotos Client Api"));
}

app.UseHttpsRedirection();

app.MapPhotoFolderEndpoints();
app.MapPhotoAlbumEndpoints();
app.MapPhotosEndpoints();

await app.RunAsync().ConfigureAwait(false);

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public partial class Program
{
    protected Program() { }
}
