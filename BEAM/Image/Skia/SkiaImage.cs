using SkiaSharp;
using System;
using System.IO;

namespace BEAM.Image.Skia;

/// <summary>
/// Decodes image data for the specified filename and value type T.
/// Supported image file formats include BMP, HEIF, JPEG, PNG and others:
/// https://learn.microsoft.com/en-us/dotnet/api/skiasharp.skencodedimageformat
/// TIFF image files are not supported by Skia, and thus SkiaSharp:
/// https://github.com/mono/SkiaSharp/issues/433.
/// </summary>
public class SkiaImage<T> : IMemoryImage, ITypedImage<T>
{
    protected SKBitmap? Data { get; set; }
    protected Func<long, T> GetValue { get; init; }
    protected Func<int, int> GetColor { get; init; }

    /// <summary>
    /// This image's dimensions, meaning its length, width and channel count.
    /// </summary>
    public ImageShape Shape { get; init; }

    /// <summary>
    /// Creates a new Skia image from a base SKBitmap.
    /// </summary>
    /// <param name="bmp">The base bitmap</param>
    public SkiaImage(SKBitmap bmp)
    {
        var width = bmp.Width;
        var height = bmp.Height;
        var channels = bmp.BytesPerPixel / bmp.ColorType.SizeOf();

        Data = bmp;
        GetValue = bmp.CreateValueGetter<T>();
        GetColor = bmp.CreateColorChannelDecoder();

        Shape = new ImageShape(width, height, channels);
        Layout = new YxzImageMemoryLayout(Shape);
    }

    /// <summary>
    /// Creates a new Skia image from a supported file found under the supplied path.
    /// </summary>
    /// <param name="stream">The file stream to decode the bitmap from</param>
    public SkiaImage(Stream stream) : this(SKBitmap.Decode(stream))
    {
    }

    /// <summary>
    /// Creates a new Skia image from a supported file found under the supplied path.
    /// Supported files: .png
    /// </summary>
    /// <param name="filepath">The path to the picture.</param>
    public SkiaImage(string filepath) : this(SKBitmap.Decode(filepath))
    {
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        if (Data != null)
        {
            Data.Dispose();
            Data = null;
        }
    }

    public double GetPixel(long x, long y, int channel)
    {
        var val = this[x, y, channel];
        return (double)Convert.ChangeType(val, typeof(double))!;
    }

    public double[] GetPixel(long x, long y)
    {
        var values = new double[Shape.Channels];
        for (var i = 0; i < Shape.Channels; i++)
        {
            values[i] = GetPixel(x, y, i);
        }

        return values;
    }

    public double[] GetPixel(long x, long y, int[] channels)
    {
        var values = new double[channels.Length];
        for (var i = 0; i < channels.Length; i++)
        {
            values[i] = GetPixel(x, y, channels[i]);
        }

        return values;
    }

    public LineImage GetPixelLineData(long[] xs, long line, int[] channels)
    {
        var data = new double[xs.Length][];
        for (var i = 0; i < xs.Length; i++)
        {
            data[i] = new double[channels.Length];
        }

        for (var x = 0; x < xs.Length; x++)
        {
            for (var channelIdx = 0; channelIdx < channels.Length; channelIdx++)
            {
                data[x][channelIdx] = GetPixel(xs[x], line, channels[channelIdx]);
            }
        }

        return new LineImage(data);
    }

    public LineImage GetPixelLineData(long line, int[] channels)
    {
        var data = new double[Shape.Width][];
        for (var i = 0; i < Shape.Width; i++)
        {
            data[i] = new double[channels.Length];
        }

        for (var x = 0; x < Shape.Width; x++)
        {
            for (var channelIdx = 0; channelIdx < channels.Length; channelIdx++)
            {
                data[x][channelIdx] = GetPixel(x, line, channels[channelIdx]);
            }
        }

        return new LineImage(data);
    }

    /// <summary>
    /// The orientation pixels are being stored in memory with.
    /// </summary>
    public ImageMemoryLayout Layout { get; init; }

    /// <summary>
    /// Gets a specific pixel value's channel by coordinates-
    /// </summary>
    /// <param name="x">The pixel's width coordinate.</param>
    /// <param name="y">The pixel's height coordinate.</param>
    /// <param name="z">The pixel's channel.</param>
    public T this[long x, long y, int z] => GetValue(Layout.Flatten(x, y, GetColor(z)));
}