
namespace BEAM.Image;

/// <summary>
/// Arrangement of the contiguous image data in memory.
/// </summary>
public abstract class ImageMemoryLayout
{
  /// <summary>
  /// Image width, height and number of color channels.
  /// </summary>
  public ImageShape Shape { get; init; }

  /// <param name="shape">
  /// Image width, height and number of color channels.
  /// </param>
  protected ImageMemoryLayout(ImageShape shape)
  {
    Shape = shape;
  }

  /// <summary>
  /// Gets the flat image value index at position X, Y, and Z
  /// according to the specific image data arrangement.
  /// </summary>
  /// <param name="x">Image pixel index in range [0..W).</param>
  /// <param name="y">Image line index in range [0..H).</param>
  /// <param name="z">Image channel index in range [0..C).</param>
  public abstract long Flatten(long x, long y, int z);
}
