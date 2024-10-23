using FluentAssertions;
using Fotos.WebApp.Tests.Assets;
using System.Net.Mime;

namespace Fotos.WebApp.Tests.Adapters;

[Trait("Category", "Integration")]
public sealed class ExifMetadataTests : IClassFixture<FotoIntegrationContext>
{
    private readonly FotoIntegrationContext _context;
    public ExifMetadataTests(FotoIntegrationContext context) => _context = context;

    [Theory(DisplayName = "Extracting EXIF metadata should pass")]
    [InlineData("Adapters/test-file.jpg")]
    internal async Task Test01(string photoPath)
    {
        await using var stream = File.OpenRead(photoPath);

        var metadata = await _context.ExtractExifMetadata(stream, MediaTypeNames.Image.Jpeg);

        metadata.DateTaken.Should().Be(new DateTime(2009, 05, 22, 8, 22, 42, DateTimeKind.Utc));
    }

    [Theory(DisplayName = "Extracting EXIF metadata should pass")]
    [InlineData("Adapters/test-file-2.jpg")]
    internal async Task Test02(string photoPath)
    {
        await using var stream = File.OpenRead(photoPath);

        var metadata = await _context.ExtractExifMetadata(stream, MediaTypeNames.Image.Jpeg);

        metadata.DateTaken.Should().Be(new DateTime(2024, 04, 23, 21, 03, 28, DateTimeKind.Utc));
    }

    [Theory(DisplayName = "Extracting EXIF metadata should pass")]
    [InlineData("Adapters/test-file-3.png")]
    internal async Task Test03(string photoPath)
    {
        await using var stream = File.OpenRead(photoPath);

        var metadata = await _context.ExtractExifMetadata(stream, MediaTypeNames.Image.Png);

        metadata.DateTaken.Should().Be(default(DateTime?));
    }
}
