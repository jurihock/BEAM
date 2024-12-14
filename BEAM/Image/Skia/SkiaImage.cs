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

  public ImageShape Shape { get; init; }

  public ImageMemoryLayout Layout { get; init; }

  public T this[int x, int y, int z] => GetValue(Layout.Flatten(x, y, GetColor(z)));
  public T this[long i] => GetValue(i);

  public double GetAsDouble(long i)
  {
    return (double) Convert.ChangeType(this[i], typeof(double))!;
  }

  public double GetAsDouble(int x, int y, int z)
  {
    return (double) Convert.ChangeType(this[x, y, z], typeof(double))!;
  }

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

public class RgbSkiaImage(string filepath) : SkiaImage<byte>(filepath);