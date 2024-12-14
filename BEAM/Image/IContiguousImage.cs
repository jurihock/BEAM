using System;

namespace BEAM.Image;

public interface IContiguousImage
{
  /// <summary>
  /// Image width, height and number of color channels.
  /// </summary>
  ImageShape Shape { get; }

  /// <summary>
  /// Returns a pixel channel's value, converted to a double.
  /// </summary>
  /// <param name="x">The pixel's width coordinate.</param>
  /// <param name="y">The pixel's height coordinate.</param>
  /// <param name="z">The pixel's channel.</param>
  /// <returns>The specified pixel-channel value as a double.</returns>
  double GetAsDouble(long x, long y, int z);

  /// <summary>
  /// Arrangement of the image values in memory.
  /// </summary>
  ImageMemoryLayout Layout { get; }

  /// <summary>
  /// Returns a pixel channel's value, converted to a double.
  /// </summary>
  /// <param name="i">The pixel's position in a one dimensional array according to the <see cref="ImageMemoryLayout"/>.</param>
  /// <returns>The specified pixel-channel value as a double.</returns>
  double GetAsDouble(long i);
}

/// <summary>
/// Indicates an image stored in a contiguous memory block.
/// </summary>
/// <typeparam name="T">
/// Type of the image value, e.g. byte, int, float, or double.
/// </typeparam>
public interface IContiguousImage<T> : IContiguousImage
{
  /// <summary>
  /// Gets image value of type T at position I.
  /// </summary>
  /// <param name="i">Flat image value index in range [0..W*H*C).</param>
  T this[long i] { get; }

  /// <summary>
  /// Gets image value of type T at position X, Y, and Z.
  /// </summary>
  /// <param name="x">Image pixel index in range [0..W).</param>
  /// <param name="y">Image line index in range [0..H).</param>
  /// <param name="z">Image channel index in range [0..C).</param>
  T this[long x, long y, int z] { get; }
  
  
}
