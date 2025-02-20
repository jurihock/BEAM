using System;
using System.IO;
using BEAM.ImageSequence;
using SkiaSharp;

namespace BEAM.Exporter;

public static class PngExporter
{
    private const long MaxHeight = 4096;
    
    public static void ExportToPng(string path, TransformedSequence sequence)
    {
        Console.WriteLine($"Exporting to {path}");
        var shape = sequence.Shape;
        Console.WriteLine($"{shape.Height / MaxHeight}");
        for (var i = 0; i <= shape.Height / MaxHeight; i++)
        {
            Console.WriteLine($"Exporting to {path} ({i})");
            var height = i - 1 == shape.Height / MaxHeight ? MaxHeight : shape.Height % MaxHeight;
            var bitmap = new SKBitmap((int) shape.Width, (int) height, true);
            for (var j = 0; j < MaxHeight && j + i * MaxHeight < shape.Height; j++)
            {
                Console.WriteLine($"Calculating {j}");
                for (var k = 0; k < shape.Width; k++)
                {
                    var data = sequence.GetPixel(k, j);
                    bitmap.SetPixel(k, j,
                        data.Length == 4
                            ? new SKColor((byte)data[0], (byte)data[1], (byte)data[2], (byte)data[3])
                            : new SKColor((byte)data[0], (byte)data[1], (byte)data[2], 255));
                }
            }

            using var image = SKImage.FromBitmap(bitmap);
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            { 
                using (var stream = File.OpenWrite(Path.Combine(path, $"{i:D8}.png")))
                {
                    data.SaveTo(stream);
                }
            }
            Console.WriteLine($"Exported {i:D8} to {path}");
        }
        Console.WriteLine($"Finished exporting to {path}");
    }
}