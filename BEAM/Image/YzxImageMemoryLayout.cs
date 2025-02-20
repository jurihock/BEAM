using System.Runtime.CompilerServices;

namespace BEAM.Image;

/// <summary>
/// YZX Cube layout in image data.
/// Data is stored in y pos order, then channel number order, lastly x pos order.
/// </summary>
public sealed class YzxImageMemoryLayout : ImageMemoryLayout
{
    private readonly long Y;
    private readonly long Z;

    public YzxImageMemoryLayout(ImageShape shape) : base(shape)
    {
        Y = 1L * Shape.Channels * Shape.Width;
        Z = 1L * Shape.Width;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override long Flatten(long x, long y, int z) => x + y * Y + z * Z;
}