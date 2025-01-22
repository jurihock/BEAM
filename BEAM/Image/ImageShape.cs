using System;

namespace BEAM.Image;

/// <summary>
/// Image width, height and number of color channels.
/// </summary>
public readonly struct ImageShape : IEquatable<ImageShape>
{
  /// <summary>
  /// Number of available image pixels.
  /// </summary>
  public long Width { get; }

  /// <summary>
  /// Number of available image lines.
  /// </summary>
  public long Height { get; }

  /// <summary>
  /// Number of available image color channels.
  /// </summary>
  public int Channels { get; }

  /// <summary>
  /// Product of image width and height.
  /// </summary>
  public long Area { get; }

  /// <summary>
  /// Product of image width, height, and channels.
  /// </summary>
  public long Volume { get; }

  /// <summary>
  /// Generates a new instance from the 3 base dimensions.
  /// </summary>
  /// <param name="width">The images number of pixels per line.</param>
  /// <param name="height">The images number of vertical lines.</param>
  /// <param name="channels">The number of channels per pixel.</param>
  public ImageShape(long width, long height, int channels)
  {
    Width = width;
    Height = height;
    Channels = channels;

    Area = (long)Width * (long)Height;
    Volume = (long)Width * (long)Height * (long)Channels;
  }

  /// <summary>
  /// Generates a new instance by copying the values of an older instance.
  /// </summary>
  /// <param name="other">The instance whose values will be copied.</param>
  public ImageShape(ImageShape other)
  {
    Width = other.Width;
    Height = other.Height;
    Channels = other.Channels;

    Area = other.Area;
    Volume = other.Volume;
  }

  public static bool operator ==(ImageShape left, ImageShape right) => left.Equals(right);
  public static bool operator !=(ImageShape left, ImageShape right) => !left.Equals(right);

  public bool Equals(ImageShape other)
  {
    if (this.Width != other.Width)
    {
      return false;
    }

    if (this.Height != other.Height)
    {
      return false;
    }

    if (this.Channels != other.Channels)
    {
      return false;
    }

    return true;
  }

  public override bool Equals(object? obj)
  {
    if (obj is ImageShape other)
    {
      return other.Equals(this);
    }

    return false;
  }

  public override string ToString()
  {
    return $"{Width}x{Height}x{Channels}";
  }

  public override int GetHashCode()
  {
    return $"{Width}x{Height}x{Channels}".GetHashCode();
  }
}
