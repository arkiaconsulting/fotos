using Fotos.Client.Api.Photos;
using SkiaSharp;

namespace Fotos.Client.Api.Adapters;

internal sealed class SkiaSharpImageProcessing
{
    public static async Task<Stream> CreateThumbnail(PhotoBinary photo)
    {
        await Task.CompletedTask;

        using var originalStream = new SKManagedStream(photo.Content);
        using var originalImage = SKImage.FromEncodedData(originalStream);

        var originalWidth = originalImage.Width;
        var originalHeight = originalImage.Height;

        var aspectRatio = (float)originalWidth / originalHeight;
        int newWidth, newHeight;

        if (originalWidth > originalHeight)
        {
            newWidth = Constants.ThumbnailMaxWidth;
            newHeight = (int)(Constants.ThumbnailMaxWidth / aspectRatio);
        }
        else
        {
            newHeight = Constants.ThumbnailMaxHeight;
            newWidth = (int)(Constants.ThumbnailMaxHeight * aspectRatio);
        }

        using var bitmap = SKBitmap.FromImage(originalImage);
        using var resizedBitmap = bitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.Medium);
        using var image = SKImage.FromBitmap(resizedBitmap);

        var encodingType = photo.MimeType switch
        {
            "image/jpeg" or "image/jpg" => SKEncodedImageFormat.Jpeg,
            "image/png" => SKEncodedImageFormat.Png,
            _ => throw new NotSupportedException($"MIME type {photo.MimeType} is not supported")
        };
        using var data = image.Encode(encodingType, 100);

        var thumbnailStream = new MemoryStream();
        data.SaveTo(thumbnailStream);
        thumbnailStream.Position = 0;

        return thumbnailStream;
    }
}
