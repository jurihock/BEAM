// (c) Paul Stier, 2025

using System;

namespace BEAM.Image;

public class LineImage(IContiguousImage baseImage, long line) : IContiguousImage
{
    public ImageShape Shape => new(baseImage.Shape.Width, 1, baseImage.Shape.Channels);

    public ImageMemoryLayout Layout => baseImage.Layout;

    public double GetAsDouble(long i)
    {
        return baseImage.GetAsDouble(i);
    }

    public double GetAsDouble(long x,long y, int z)
    {
        return baseImage.GetAsDouble(x, line, z);
    }

    public double[] GetPixel(long x, int[] channels)
    {
        var result = new double[channels.Length];

        for (var i = 0; i < channels.Length; i++)
        {
            result[i] = GetAsDouble(x, line, channels[i]);
        }

        return result;
    }

    public double[,] GetChannels(int[] channels)
    {
        var result = new double[Shape.Width, channels.Length];
        for (var x = 0; x < Shape.Width; x++)
        {
            for (var channelIdx = 0; channelIdx < channels.Length; channelIdx++)
            {
                result[x, channelIdx] = GetAsDouble(x, line, channels[channelIdx]);
            }
        }

        return result;
    }
}