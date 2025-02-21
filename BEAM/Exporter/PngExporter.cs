using System.IO;
using Avalonia.Platform.Storage;
using BEAM.ImageSequence;
using BEAM.Models.Log;
using BEAM.Renderer;
using SkiaSharp;

namespace BEAM.Exporter;

public static class PngExporter
{
    private const long MaxHeight = 4096;
    
    public static void Export(IStorageFolder? path, string name, TransformedSequence sequence, SequenceRenderer renderer)
    {
        if (path is null)
        {
            return;
        }
        Directory.CreateDirectory(Path.Combine(path.Path.AbsolutePath, name));
        var shape = sequence.Shape;
        for (var i = 0; i <= shape.Height / MaxHeight; i++)
        {
            var height = i - 1 == shape.Height / MaxHeight ? shape.Height % MaxHeight : MaxHeight;
            var bitmap = new SKBitmap((int) shape.Width, (int) height, true);
            for (var j = 0; j < MaxHeight && j + i * MaxHeight < shape.Height; j++)
            {
                for (var k = 0; k < shape.Width; k++)
                {
                    var data = renderer.RenderPixel(sequence, k, j + i * MaxHeight);
                    bitmap.SetPixel(k, j,
                        data.Length == 4
                            ? new SKColor((byte)data[3], (byte)data[1], (byte)data[2], (byte)data[3])
                            : new SKColor((byte)data[3], (byte)data[1], (byte)data[2], 255));
                }
            }

            using var image = SKImage.FromBitmap(bitmap);
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            { 
                using (var stream = File.OpenWrite(Path.Combine(path.Path.AbsolutePath, name, $"{i:D8}.png")))
                {
                    data.SaveTo(stream);
                }
            }
        }
        Logger.GetInstance().LogMessage($"Finished exporting sequence {sequence.GetName()} as Png to {path.Path.AbsolutePath}");
    }
}