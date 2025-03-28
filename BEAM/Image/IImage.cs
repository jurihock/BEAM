using System;

namespace BEAM.Image;

public interface IImage : IDisposable
{
    /// <summary>
    /// The shape of the image, containing width, height and channel count
    /// </summary>
    ImageShape Shape { get; }

    /// <summary>
    /// Gets the value of a single channel of a pixel.
    /// </summary>
    /// <param name="x">The x coordinate of the pixel</param>
    /// <param name="y">The y coordinate of the pixel</param>
    /// <param name="channel"></param>
    /// <returns>THe channels value as a double</returns>
    double GetPixel(long x, long y, int channel);

    /// <summary>
    /// Gets all channel values of a pixel.
    /// </summary>
    /// <param name="x">The x coordinate of the pixel</param>
    /// <param name="y">The y coordinate of the pixel</param>
    /// <returns>The channel values as doubles</returns>
    double[] GetPixel(long x, long y);

    /// <summary>
    /// Gets channel values of a pixel.
    /// </summary>
    /// <param name="x">The x coordinate of the pixel</param>
    /// <param name="y">The y coordinate of the pixel</param>
    /// <param name="channels">The channels of the pixel to get their value from</param>
    /// <returns>The requested channel values</returns>
    double[] GetPixel(long x, long y, int[] channels);

    /// <summary>
    /// Returns the pixel data for a single line.
    /// </summary>
    /// <param name="line">The line of the image to get</param>
    /// <param name="channels">The requested channels</param>
    /// <returns>The pixel data as an IImage</returns>
    LineImage GetPixelLineData(long line, int[] channels);

    /// <summary>
    /// Returns the pixel data for a single line.
    /// </summary>
    /// <param name="xs">The column of the pixel use get</param>
    /// <param name="line">The line of the image to get</param>
    /// <param name="channels">The requested channels</param>
    /// <returns>The pixel data as an IImage</returns>
    LineImage GetPixelLineData(long[] xs, long line, int[] channels);
}