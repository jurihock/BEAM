using System;
using BEAM.Exceptions;

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
    
    /// <summary>
    /// Given the intensities of all channels of the pixel, three channels are chosen for the R, G and B value.
    /// The result is the ARGB-color values as a byte array.
    /// </summary>
    /// <param name="channels"></param>
    /// <param name="displayParameters"></param>
    /// <returns></returns>
    /// <exception cref="ChannelException"></exception>
    public override byte[] RenderPixel(double[] channels, double[] displayParameters)
    {
        int channelAmount = channels.Length;
        if (Math.Max(Math.Max(ChannelBlue, ChannelGreen), ChannelRed) > channelAmount)
        {
            throw new ChannelException("Chosen Channels are larger than the number of channels");
        }

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