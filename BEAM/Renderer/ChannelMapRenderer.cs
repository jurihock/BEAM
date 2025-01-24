using System;
using System.Runtime.Intrinsics;
using BEAM.Exceptions;
using BEAM.ImageSequence;
using BEAM.Profiling;

namespace BEAM.Renderer;

/// <summary>
/// Renderer that maps n given channels to an ARGB color value.
/// For this three channel numbers i, j, k < n (first channel is 0) are given.
/// Red is set to the intensity of the ith channel, Green to the jth channel, Blue to the kth channel.
/// </summary>
public class ChannelMapRenderer : SequenceRenderer
{
    public ChannelMapRenderer(int minimumOfIntensityRange, int maximumOfIntensityRange,
        int channelRed, int channelGreen, int channelBlue)
        : base(minimumOfIntensityRange, maximumOfIntensityRange)
    {
        ChannelRed = channelRed;
        ChannelGreen = channelGreen;
        ChannelBlue = channelBlue;
    }

    public int ChannelRed { get; set; }
    public int ChannelGreen { get; set; }
    public int ChannelBlue { get; set; }

    public override byte[] RenderPixel(Sequence sequence, long x, long y)
    {
        var channels = sequence.GetPixel(x, y);

        var colors = NormailizeIntensity(Vector256.Create([
            channels[ChannelRed],
            channels[ChannelGreen],
            channels[ChannelBlue],
            0
        ]));

        byte[] color =
        [
            255,
            (byte)colors[0],
            (byte)colors[1],
            (byte)colors[2]
        ];

        return color;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sequence"></param>
    /// <param name="xs"></param>
    /// <param name="y"></param>
    /// <returns>[x, argb]</returns>
    public override byte[,] RenderPixels(Sequence sequence, long[] xs, long y)
    {
        var data = new byte[xs.Length, 4];
        var img = sequence.GetPixelLineData(xs, y, [ChannelRed, ChannelGreen, ChannelBlue]);

        for (var x = 0; x < xs.Length; x++)
        {
            var colors = NormailizeIntensity(Vector256.Create([
                img.GetPixel(x, 0, ChannelRed),
                img.GetPixel(x, 0, ChannelGreen),
                img.GetPixel(x, 0, ChannelBlue),
                0
            ]));

            data[x, 0] = 255;
            data[x, 1] = (byte)colors[0];
            data[x, 2] = (byte)colors[1];
            data[x, 3] = (byte)colors[2];
        }
        return data;
    }

    private Vector256<double> NormailizeIntensity(Vector256<double> intensities)
    {
        var minIntensities = Vector256.Create<double>(MinimumOfIntensityRange);
        var maxIntensities = Vector256.Create<double>(MaximumOfIntensityRange);
        var multFactor = Vector256.Create<double>(255);

        return (intensities - minIntensities) / (maxIntensities - minIntensities) * multFactor;
    }
}