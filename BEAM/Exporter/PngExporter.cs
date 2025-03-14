using System;
using System.IO;
using System.Threading;
using Avalonia.Platform.Storage;
using BEAM.ImageSequence;
using BEAM.Renderer;
using BEAM.ViewModels;
using SkiaSharp;
using Logger = BEAM.Models.Log.Logger;

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
    public static void Export(IStorageFile path, TransformedSequence sequence, SequenceRenderer renderer, ExportProgressWindowViewModel vm)
    {
        var i = 0;
        Directory.CreateDirectory(path.Path.AbsolutePath);
        CancellationToken ctx = vm.GetCancellationToken();
        var shape = sequence.Shape;
        var steps = MaxHeight/10;
        for (; i < shape.Height / MaxHeight; i++)
        {
            ctx.ThrowIfCancellationRequested();
            var bitmap = new SKBitmap((int)shape.Width, (int)MaxHeight, SKColorType.Bgra8888, SKAlphaType.Opaque);
            for (var j = 0; j < MaxHeight && j + i * MaxHeight < shape.Height; j++)
            {
                ctx.ThrowIfCancellationRequested();
                for (var k = 0; k < shape.Width; k++)
                {
                    var data = renderer.RenderPixel(sequence, k, j + i * MaxHeight);
                    bitmap.SetPixel(k, j, new SKColor(data.R, data.G, data.B, 255));
                }
                if(j%steps == 0)
                {
                    vm.ActionProgress = (byte)(Math.Round(((i  * MaxHeight + j) / ((double) shape.Height + 1)) * 100));
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
            var bitmap = new SKBitmap((int)shape.Width, (int)height, SKColorType.Bgra8888, SKAlphaType.Opaque);
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
                    ctx.ThrowIfCancellationRequested();
                    data.SaveTo(stream);
                }
            }
        }

        vm.Close();
        Logger.GetInstance().LogMessage($"Finished exporting sequence {sequence.GetName()} as Png to {path.Path.AbsolutePath}");
    }

    /// <summary>
    /// Cleans up the files created during the export if it was prematurely canceled
    /// </summary>
    /// <param name="folder">The folder within which the original export was meant to take place.</param>
    public static void Cleanup(IStorageFile folder)
    {
        Directory.Delete(folder.Path.AbsolutePath, true);
    }
}