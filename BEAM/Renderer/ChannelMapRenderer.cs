using System;
using BEAM.Exceptions;
using BEAM.ImageSequence;

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

    int ChannelRed { get; init; }
    int ChannelGreen { get; init; }
    int ChannelBlue { get; init; }

    public override byte[] RenderPixel(Sequence sequence, long x, long y)
    {
        var channels = sequence.GetPixel(x, y);

        byte[] color =
        [
            255,
            (byte)(NormalizeIntensity(channels[ChannelRed]) * 255),
            (byte)(NormalizeIntensity(channels[ChannelGreen]) * 255),
            (byte)(NormalizeIntensity(channels[ChannelBlue]) * 255),
        ];

        return color;
    }
}