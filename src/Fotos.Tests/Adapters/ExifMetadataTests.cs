using FluentAssertions;
using Fotos.Tests.Assets;
using System.Net.Mime;

namespace Fotos.Tests.Adapters;

[Trait("Category", "Integration")]
public sealed class ExifMetadataTests : IClassFixture<FotoIntegrationContext>
{
    private readonly FotoIntegrationContext _context;
    public ExifMetadataTests(FotoIntegrationContext context) => _context = context;

    [Theory(DisplayName = "Extracting EXIF metadata should pass")]
    [InlineData("Backend/Adapters/test-file.jpg")]
    internal async Task Test01(string photoPath)
    {
        await using var stream = File.OpenRead(photoPath);

        var metadata = await _context.ExtractExifMetadata(stream, MediaTypeNames.Image.Jpeg);

        metadata.DateTaken.Should().Be(new DateTime(2009, 05, 22, 8, 22, 42, DateTimeKind.Utc));
    }

    [Theory(DisplayName = "Extracting EXIF metadata should pass")]
    [InlineData("Backend/Adapters/test-file-2.jpg")]
    internal async Task Test02(string photoPath)
    {
        await using var stream = File.OpenRead(photoPath);

        var metadata = await _context.ExtractExifMetadata(stream, MediaTypeNames.Image.Jpeg);

        metadata.DateTaken.Should().Be(new DateTime(2024, 04, 23, 21, 03, 28, DateTimeKind.Utc));
    }

    [Theory(DisplayName = "Extracting EXIF metadata should pass")]
    [InlineData("Backend/Adapters/test-file-3.png")]
    internal async Task Test03(string photoPath)
    {
        await using var stream = File.OpenRead(photoPath);

        var metadata = await _context.ExtractExifMetadata(stream, MediaTypeNames.Image.Png);

        metadata.DateTaken.Should().Be(default(DateTime?));
    }

    [Theory(DisplayName = "Extracting EXIF metadata should pass")]
    [InlineData("Backend/Adapters/test-file-4.jpg")]
    internal async Task Test04(string photoPath)
    {
        await using var stream = File.OpenRead(photoPath);

        var metadata = await _context.ExtractExifMetadata(stream, MediaTypeNames.Image.Jpeg);

        metadata.DateTaken.Should().Be(new DateTime(2022, 08, 09, 14, 41, 26, DateTimeKind.Utc));
        metadata.CameraBrand.Should().Be("NIKON CORPORATION");
        metadata.CameraModel.Should().Be("NIKON D80");
        metadata.Iso.Should().Be(320);
        metadata.Width.Should().Be(3872);
        metadata.Height.Should().Be(2592);
        metadata.Aperture.Should().Be(8);
        metadata.FocalLength.Should().Be(135);
        metadata.IsFlash.Should().BeFalse();
        metadata.ExposureTime.Should().Be("1/250 sec");
        metadata.Lens.Should().Be("18-135mm f/3.5-5.6");
        metadata.Quality.Should().Be("Fine");
    }

    [Theory(DisplayName = "Extracting EXIF metadata should pass")]
    [InlineData("Backend/Adapters/test-file-5.jpg")]
    internal async Task Test05(string photoPath)
    {
        await using var stream = File.OpenRead(photoPath);

        var metadata = await _context.ExtractExifMetadata(stream, MediaTypeNames.Image.Jpeg);

        metadata.IsFlash.Should().BeTrue();
    }

    [Theory(DisplayName = "Extracting EXIF metadata should pass")]
    [InlineData("Backend/Adapters/test-file-iphone.jpeg")]
    internal async Task Test06(string photoPath)
    {
        await using var stream = File.OpenRead(photoPath);

        var metadata = await _context.ExtractExifMetadata(stream, MediaTypeNames.Image.Jpeg);

        metadata.IsFlash.Should().BeFalse();
    }
}
