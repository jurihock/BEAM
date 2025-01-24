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
        /*
        renderer = new HeatMapRendererRB(0, 1, 1);
        */

        startX = Math.Clamp(startX, 0, sequence.Shape.Width);
        endX = Math.Clamp(endX, 0, sequence.Shape.Width);
        startLine = Math.Clamp(startLine, 0, sequence.Shape.Height);
        endLine = Math.Clamp(endLine, 0, sequence.Shape.Height);

        width = (int)Math.Clamp(width, 0, endX - startX);
        height = (int)Math.Clamp(height, 0, endLine - startLine);

        BgraBitmap bitmap = new(width, height);

        Parallel.For(0, height, j =>
        {
            var line = startLine + j * (endLine - startLine) / height;

            var xs = new long[width];
            for (var i = 0; i < width; i++)
            {
                xs[i] = startX + i * (endX - startX) / width;
            }

            var data = renderer.RenderPixels(sequence, xs, line);

            var span = bitmap.GetPixelSpan();
            var pixels = MemoryMarshal.Cast<byte, BGRA>(span);

            for (var i = 0; i < width; i++)
            {
                pixels[j * width + i] = new BGRA()
                {
                    R = data[i, 1],
                    G = data[i, 2],
                    B = data[i, 3],
                    A = data[i, 0]
                };
            }
        });

        return bitmap;
    }
}