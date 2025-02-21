using System;
using NP.Utilities;

namespace BEAM.Image;

/// <summary>
/// An image with height 1. Used as a way to transfer single line information.
/// </summary>
public class LineImage : IImage
{
    private readonly double[][] _data;

    /// <summary>
    /// The data of the line.
    /// </summary>
    /// <param name="data">The data with layout data[x pos][channel]</param>
    public LineImage(double[][] data)
    {
        _data = data;
        if (data.IsNullOrEmpty() || data[0].IsNullOrEmpty())
        {
            throw new ArgumentException("Line image data cannot be empty");
        }
    }

    public ImageShape Shape => new(_data.Length, 1, _data[0].Length);

    public double GetPixel(long x, long y, int channel)
    {
        if (y != 0)
            throw new ArgumentOutOfRangeException(nameof(y), y, $"{y} can only be 0 since access to LineImage");
        if (x < 0 || x >= Shape.Width)
            throw new ArgumentOutOfRangeException(nameof(x), x, $"{x} is out of range [0, {Shape.Width})");
        if (channel < 0 || channel >= Shape.Channels)
            throw new ArgumentOutOfRangeException(nameof(channel), channel,
                $"{channel} is out of range [0, {Shape.Channels})");

        return _data[x][channel];
    }

    public double[] GetPixel(long x, long y)
    {
        if (y != 0)
            throw new ArgumentOutOfRangeException(nameof(y), y, $"{y} can only be 0 since access to LineImage");
        if (x < 0 || x >= Shape.Width)
            throw new ArgumentOutOfRangeException(nameof(x), x, $"{x} is out of range [0, {Shape.Width})");

        return _data[x];
    }

    public double[] GetPixel(long x, long y, int[] channels)
    {
        if (y != 0)
            throw new ArgumentOutOfRangeException(nameof(y), y, $"{y} can only be 0 since access to LineImage");

        var values = new double[channels.Length];
        for (var i = 0; i < channels.Length; i++)
        {
            values[i] = GetPixel(x, y, channels[i]);
        }

        return values;
    }

    /// <summary>
    /// Returns this LineImage, if the line is 0.
    /// Warning: DO NOT USE THIS METHOD
    /// The channels of this LineImage are not altered, as they might have already been altered from the original sequence.
    /// It is not intended to call this method on the LineImage.
    /// </summary>
    /// <param name="line"></param>
    /// <param name="channels"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public LineImage GetPixelLineData(long line, int[] channels)
    {
        if (line != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(line), line, $"{line} can only be 0 for LineImage");
        }

        return this;
    }

    public LineImage GetPixelLineData(long[] xs, long line, int[] channels)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        // class does not manage additional resources -> no need to dispose
        GC.SuppressFinalize(this);
    }
}