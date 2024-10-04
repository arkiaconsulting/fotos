using FluentValidation;
using Fotos.WebApp.Features.PhotoAlbums;
using Fotos.WebApp.Features.PhotoFolders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("fotos", new() { Title = "Fotos" });
    options.IncludeXmlComments(typeof(Program).Assembly.Location.Replace("dll", "xml", StringComparison.OrdinalIgnoreCase));
    options.NonNullableReferenceTypesAsRequired();
});
builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => options.RouteTemplate = "{documentName}/openapi.json");
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPhotoFolderEndpoints();
app.MapPhotoAlbumEndpoints();

await app.RunAsync().ConfigureAwait(false);

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "<Pending>")]
public partial class Program
{
    protected Program() { }
}
