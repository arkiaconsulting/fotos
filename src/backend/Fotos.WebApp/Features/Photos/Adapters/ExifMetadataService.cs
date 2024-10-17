﻿using Fotos.WebApp.Types;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using System.Net.Mime;

namespace Fotos.WebApp.Features.Photos.Adapters;

internal sealed class ExifMetadataService
{
    public static async Task<ExifMetadata> Extract(Stream photo, string mimeType)
    {
        await Task.CompletedTask;

        return mimeType switch
        {
            MediaTypeNames.Image.Jpeg or "image/jpg" => ExtractExifFromJpeg(photo),
            MediaTypeNames.Image.Png => ExtractExifFromPng(photo),
            _ => throw new NotSupportedException($"MIME type {mimeType} is not supported")
        };
    }

    private static ExifMetadata ExtractExifFromJpeg(Stream photo)
    {
        var readers = new IJpegSegmentMetadataReader[] { new JpegReader(), new ExifReader() };
        var directories = JpegMetadataReader.ReadMetadata(photo, readers);
        var date = directories.OfType<ExifIfd0Directory>().FirstOrDefault()?
            .TryGetDateTime(ExifDirectoryBase.TagDateTime, out var exifDate) == true ? exifDate : default(DateTime?);

        return new(date);
    }

    private static ExifMetadata ExtractExifFromPng(Stream _) => new();
}