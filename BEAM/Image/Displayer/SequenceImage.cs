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
        using var _ = Timer.Start();

        BgraBitmap bitmap = new(width, height);

        Parallel.For(0, height, j =>
        {
            //using var _abc = Timer.Start($"{j}");
            var span = bitmap.GetPixelSpan();
            var pixels = MemoryMarshal.Cast<byte, BGRA>(span);
            var line = startLine + j * (endLine - startLine) / height;

            var image = sequence.GetImage((int) (line / sequence.SingleImageHeight));
            line = line % sequence.SingleImageHeight;
            //var data = image.GetChannels([0, 1, 2, 3]);

            var data = new double[4];
            for (var i = 0; i < width; i++)
            {
                var x = startX + i * (endX - startX) / width;
                //data = image.GetPixel(x, [0, 1, 2, 3]);
                pixels[j * width + i] = new BGRA()
                {
                    B = (byte)image.GetAsDouble(x, line, 0),
                    G = (byte)image.GetAsDouble(x, line, 1),
                    R = (byte)image.GetAsDouble(x, line, 2),
                    A = 255
                };
            }
        });

        return bitmap;
    }
}