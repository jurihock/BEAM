using System;
using System.IO;
using BEAM.Exceptions;
using BEAM.ImageSequence;

namespace BEAM.Renderer;

/// <summary>
/// General abstract class for a HeatMapRenderer.
/// It implements the function to Render a pixel.
/// The concrete function, that converts the channel intensity to a color is implemented by the subclasses.
/// </summary>
public abstract class HeatMapRenderer(int minimumOfIntensityRange, int maximumOfIntensityRange, int channel)
    : SequenceRenderer(minimumOfIntensityRange, maximumOfIntensityRange)
{
    /// <summary>
    /// The channel that is used in the HeatMap for the intensity of the HeatMap.
    /// </summary>
    public int Channel { get; set; } = channel;

    public override byte[] RenderPixel(Sequence sequence, long x, long y)
    {
        return GetColor(sequence.GetPixel(x, y, Channel), MinimumOfIntensityRange, MaximumOfIntensityRange);
    }

    public override byte[,] RenderPixels(Sequence sequence, long[] xs, long y)
    {
        var data = new byte[xs.Length, 4];
        var img = sequence.GetPixelLineData(xs, y, [Channel]);

        // TODO: SIMD
        for (var i = 0; i < xs.Length; i++)
        {
            var color = GetColor(img.GetPixel(i, 0, 0), MinimumOfIntensityRange, MaximumOfIntensityRange);
            data[i, 0] = color[0];
            data[i, 1] = color[1];
            data[i, 2] = color[2];
            data[i, 3] = color[3];
        }

        return data;
    }

    /// <summary>
    /// The specific function that converts the intensity of a channel into a Color.
    /// Is implemented by a subclass.
    /// </summary>
    /// <param name="value">The intensity of the chosen channel</param>
    /// <param name="min">The highest intensity of the channel, that is displayed as the coldest intensity.</param>
    /// <param name="max">The lowest intensity of the channel, that is displayed as the highest intensity.</param>
    /// <returns>The ARGB values of the final Color to be displayed.
    /// (A, R, G, B) each color from 0 - 255. A = 0 : fully transparent</returns>
    protected abstract byte[] GetColor(double value, double min, double max);
}