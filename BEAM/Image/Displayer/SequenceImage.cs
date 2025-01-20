// (c) Paul Stier, 2025

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using BEAM.Image.Bitmap;
using BEAM.ImageSequence;
using SkiaSharp;
using Timer = BEAM.Profiling.Timer;

namespace BEAM.Image.Displayer;

public class SequenceImage(Sequence sequence)
{
    public SKBitmap GetImage(long startX, long endX, long startLine, long endLine, int width, int height)
    {
        startX = Math.Clamp(startX, 0, sequence.Shape.Width);
        endX = Math.Clamp(endX, 0, sequence.Shape.Width);
        startLine = Math.Clamp(startLine, 0, sequence.Shape.Height);
        endLine = Math.Clamp(endLine, 0, sequence.Shape.Height);

        width = (int)Math.Clamp(width, 0, endX - startX);
        height = (int)Math.Clamp(height, 0, endLine - startLine);

        /*Console.WriteLine(
            $"startX: {startX} endX: {endX} startLine: {startLine} endLine: {endLine} Width: {width}, Height: {height}");*/

        using var _ = Timer.Start();

        Console.WriteLine($"width={width}, height={height}");

        BgraBitmap bitmap = new(width, height);

        Parallel.For(0, height, j =>
        {
            //using var _abc = Timer.Start($"{j}");
            var span = bitmap.GetPixelSpan();
            var pixels = MemoryMarshal.Cast<byte, BGRA>(span);
            var line = startLine + j * (endLine - startLine) / height;

            var image = sequence.GetLineImage(line);
            //var data = image.GetChannels([0, 1, 2, 3]);

            var data = new double[4];
            for (var i = 0; i < width; i++)
            {
                var x = startX + i * (endX - startX) / width;
                data = image.GetPixel(x, [0, 1, 2, 3]);
                pixels[j * width + i] = new BGRA()
                {
                    B = (byte)data[0],
                    G = (byte)data[1],
                    R = (byte)data[2],
                    A = (byte)data[3],
                };
            }
        });

        return bitmap;
    }
}