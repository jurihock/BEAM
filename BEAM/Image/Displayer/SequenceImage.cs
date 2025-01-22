// (c) Paul Stier, 2025

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BEAM.Image.Bitmap;
using BEAM.ImageSequence;
using BEAM.Renderer;
using SkiaSharp;

namespace BEAM.Image.Displayer;

public class SequenceImage(Sequence sequence)
{
    public SKBitmap GetImage(long startX, long endX, long startLine, long endLine, int width, int height)
    {
        // TODO: change
        SequenceRenderer renderer = new ChannelMapRenderer(0, 255, 2, 1, 0);

        startX = Math.Clamp(startX, 0, sequence.Shape.Width);
        endX = Math.Clamp(endX, 0, sequence.Shape.Width);
        startLine = Math.Clamp(startLine, 0, sequence.Shape.Height);
        endLine = Math.Clamp(endLine, 0, sequence.Shape.Height);

        width = (int)Math.Clamp(width, 0, endX - startX);
        height = (int)Math.Clamp(height, 0, endLine - startLine);

        BgraBitmap bitmap = new(width, height);

        Parallel.For(0, height, j =>
        {
            //using var _abc = Timer.Start($"{j}");
            var span = bitmap.GetPixelSpan();
            var pixels = MemoryMarshal.Cast<byte, BGRA>(span);
            var line = startLine + j * (endLine - startLine) / height;

            for (var i = 0; i < width; i++)
            {
                var x = startX + i * (endX - startX) / width;
                var data = renderer.RenderPixel(sequence, x, line);
                pixels[j * width + i] = new BGRA()
                {
                    R = data[1],
                    G = data[2],
                    B = data[3],
                    A = data[0]
                };
            }
        });

        return bitmap;
    }
}