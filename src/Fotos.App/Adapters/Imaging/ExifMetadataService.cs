using Fotos.App.Domain;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.Makernotes;
using MetadataExtractor.Formats.Jpeg;
using System.Diagnostics;
using System.Globalization;
using System.Net.Mime;

namespace Fotos.App.Adapters.Imaging;

internal sealed class ExifMetadataService
{
    private readonly ActivitySource _activitySource;

    public ExifMetadataService(InstrumentationConfig instrumentation) => _activitySource = instrumentation.ActivitySource;

    public async Task<ExifMetadata> Extract(Stream photo, string mimeType)
    {
        using var activity = _activitySource?.StartActivity("extract EXIF metadata");
        activity?.SetTag("mimeType", mimeType);
        activity?.SetTag("size", photo.Length);

        await Task.CompletedTask;

        var metadata = mimeType switch
        {
            MediaTypeNames.Image.Jpeg or "image/jpg" => ExtractExifFromJpeg(photo),
            MediaTypeNames.Image.Png => ExtractExifFromPng(photo),
            _ => throw new NotSupportedException($"MIME type {mimeType} is not supported")
        };

        activity?.AddEvent(new("EXIF metadata extracted"));

        return metadata;
    }

    private static ExifMetadata ExtractExifFromJpeg(Stream photo)
    {
        var readers = new IJpegSegmentMetadataReader[] { new JpegReader(), new ExifReader() };
        var directories = JpegMetadataReader.ReadMetadata(photo, readers);
        var metadata = new ExifMetadata();

        var ifd0 = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
        if (ifd0 != null)
        {
            metadata = metadata with
            {
                CameraBrand = ifd0.GetString(ExifDirectoryBase.TagMake),
                CameraModel = ifd0.GetString(ExifDirectoryBase.TagModel),
                DateTaken = ifd0.TryGetDateTime(ExifDirectoryBase.TagDateTime, out var exifDate) ? exifDate : default(DateTime?)
            };
        }

        var subIf = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
        if (subIf != null)
        {
            metadata = metadata with
            {
                Iso = subIf.TryGetInt32(ExifDirectoryBase.TagIsoEquivalent, out var iso1) ? iso1 :
                    subIf.TryGetInt32(ExifDirectoryBase.TagIsoSpeed, out var iso2) ? iso2 : default(int?),
                Width = subIf.TryGetInt32(ExifDirectoryBase.TagExifImageWidth, out var width) ? width : default(int?),
                Height = subIf.TryGetInt32(ExifDirectoryBase.TagExifImageHeight, out var height) ? height : default(int?),
                Aperture = subIf.TryGetInt32(ExifDirectoryBase.TagFNumber, out var aperture) ? aperture :
                    subIf.TryGetInt32(ExifDirectoryBase.TagAperture, out var aperture2) ? aperture2 : default(int?),
                FocalLength = subIf.TryGetInt32(ExifDirectoryBase.TagFocalLength, out var focalLength) ? focalLength : default(int?),
                IsFlash = subIf.TryGetByte(ExifDirectoryBase.TagFlash, out var flash) && flash != 0x10 && flash != 0x0,
                ExposureTime = subIf.GetDescription(ExifDirectoryBase.TagExposureTime),
                Lens = subIf.GetDescription(ExifDirectoryBase.TagLensModel)
            };
        }

        var nikon = directories.OfType<NikonType2MakernoteDirectory>().FirstOrDefault();
        if (nikon != null)
        {
            metadata = metadata with
            {
                Lens = nikon.GetDescription(NikonType2MakernoteDirectory.TagLens),
                Quality = CultureInfo.InvariantCulture.TextInfo.ToTitleCase((nikon.GetDescription(NikonType2MakernoteDirectory.TagQualityAndFileFormat) ?? string.Empty).ToLowerInvariant().Trim())
            };
        }

        return metadata;
    }

    private static ExifMetadata ExtractExifFromPng(Stream _) => new();
}
