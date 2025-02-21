using System.IO;
using Avalonia.Platform.Storage;
using BEAM.ImageSequence;
using BEAM.Models.Log;
using BEAM.Renderer;
using SkiaSharp;

namespace BEAM.Exporter;

/// <summary>
/// Provides functionality to export image sequences in the PNG format.
/// </summary>
public static class PngExporter
{
    /// <summary>
    /// The maximum height for each PNG file.
    /// </summary>
    private const long MaxHeight = 4096;

    /// <summary>
    /// Exports the given sequence to the specified path in the PNG format.
    /// </summary>
    /// <param name="path">The path where the files will be saved.</param>
    /// <param name="sequence">The sequence to be exported.</param>
    /// <param name="renderer">The renderer used for the sequence.</param>
    public static void Export(IStorageFile path, TransformedSequence sequence, SequenceRenderer renderer)
    {
        var i = 0;
        Directory.CreateDirectory(path.Path.AbsolutePath);
        var shape = sequence.Shape;
        for (; i < shape.Height / MaxHeight; i++)
        {
            var bitmap = new SKBitmap((int) shape.Width, (int) MaxHeight, SKColorType.Bgra8888, SKAlphaType.Opaque);
            for (var j = 0; j < MaxHeight && j + i * MaxHeight < shape.Height; j++)
            {
                for (var k = 0; k < shape.Width; k++)
                {
                    var data = renderer.RenderPixel(sequence, k, j + i * MaxHeight);
                    bitmap.SetPixel(k, j, new SKColor(data.R, data.G, data.B, 255));
                }
            }

            using var image = SKImage.FromBitmap(bitmap);
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                using (var stream = File.OpenWrite(Path.Combine(path.Path.AbsolutePath, $"{i:D8}.png")))
                {
                    data.SaveTo(stream);
                }
            }
        }
        if (shape.Height % MaxHeight != 0)
        {
            var height = shape.Height % MaxHeight;
            var bitmap = new SKBitmap((int) shape.Width, (int) height, SKColorType.Bgra8888, SKAlphaType.Opaque);
            for (var j = 0; j < height; j++)
            {
                for (var k = 0; k < shape.Width; k++)
                {
                    var data = renderer.RenderPixel(sequence, k, j + shape.Height - height);
                    bitmap.SetPixel(k, j, new SKColor(data.R, data.G, data.B, 255));
                }
            }

            using var image = SKImage.FromBitmap(bitmap);
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                using (var stream = File.OpenWrite(Path.Combine(path.Path.AbsolutePath, $"{i:D8}.png")))
                {
                    data.SaveTo(stream);
                }
            }
        }
        Logger.GetInstance().LogMessage($"Finished exporting sequence {sequence.GetName()} as Png to {path.Path.AbsolutePath}");
    }
}