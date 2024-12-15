using System;

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
    
    public override byte[] RenderPixel(double[] channels, double[] displayParameters)
    {
        int channelAmount = channels.Length;
        if (Math.Max(ChannelBlue, ChannelGreen, ChannelRed) > channelAmount)
        {
            throw new ArgumentException("Channel amount must be less than or equal to ChannelGreen");
        }
        byte[] color = [255, ]
    }
}