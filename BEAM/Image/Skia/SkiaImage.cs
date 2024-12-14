using SkiaSharp;
using System;

namespace BEAM.Image.Skia;

public interface ISkiaImage : IContiguousImage
{
}

/// <summary>
/// Decodes image data for the specified filename and value type T.
/// Supported image file formats include BMP, HEIF, JPEG, PNG and others:
/// https://learn.microsoft.com/en-us/dotnet/api/skiasharp.skencodedimageformat
/// TIFF image files are not supported by Skia, and thus SkiaSharp:
/// https://github.com/mono/SkiaSharp/issues/433
/// </summary>
public abstract class SkiaImage<T> : IContiguousImage<T>, IDisposable, ISkiaImage
{
  private SKBitmap? Data { get; set; }
  private Func<long, T> GetValue { get; init; }
  private Func<int, int> GetColor { get; init; }
  
  /// <summary>
  /// This image's dimensions, meaning its length, width and channel count.
  /// </summary>
  public ImageShape Shape { get; init; }

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
  
  /// <summary>
  /// Gets a specific pixel value's channel by the pixels position in a flattened one dimensional image, meaning by its position in the raw filestream.
  /// </summary>
  /// <param name="i">The pixel's position in a one dimensional array according to the <see cref="ImageMemoryLayout"/>.</param>
  public T this[long i] => GetValue(i);

  public double GetAsDouble(long i)
  {
    return (double) Convert.ChangeType(this[i], typeof(double))!;
  }

  public double GetAsDouble(long x, long y, int z)
  {
    return (double) Convert.ChangeType(this[x, y, z], typeof(double))!;
  }

  /// <summary>
  /// Creates a new Skia image from a supported file found under the supplied path.
  /// Supported files: .png
  /// </summary>
  /// <param name="filepath">The path to the picture.</param>
  public SkiaImage(string filepath)
  {
    var bmp = SKBitmap.Decode(filepath);

    var width = bmp.Width;
    var height = bmp.Height;
    var channels = bmp.BytesPerPixel / bmp.ColorType.SizeOf();

    Data = bmp;
    GetValue = bmp.CreateValueGetter<T>();
    GetColor = bmp.CreateColorChannelDecoder();

    Shape = new ImageShape(width, height, channels);
    Layout = new YxzImageMemoryLayout(Shape);
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
}


/// <summary>
/// An RGB SKia image, meaning each channel has values in the range of a byte.
/// </summary>
/// <param name="filepath">The path to the png picture.</param>
public class RgbSkiaImage(string filepath) : SkiaImage<byte>(filepath);