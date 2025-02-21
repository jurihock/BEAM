using System.Runtime.CompilerServices;

namespace BEAM.Image;

/// <summary>
/// YXZ Cube layout in image data.
/// Data is stored in y pos order, then x pos order, lastly channel number order.
/// </summary>
public sealed class YxzImageMemoryLayout : ImageMemoryLayout
{
    private readonly long _x;
    private readonly long _y;

    public YxzImageMemoryLayout(ImageShape shape) : base(shape)
    {
        _x = 1L * Shape.Channels;
        _y = 1L * Shape.Width * Shape.Channels;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override long Flatten(long x, long y, int z) => x * _x + y * _y + z;
}