using System.Runtime.CompilerServices;

namespace BEAM.Image;

/// <summary>
/// XYZ Cube layout in image data.
/// Data is stored in x pos order, then y pos order, lastly channel number order.
/// </summary>
public sealed class XyzImageMemoryLayout : ImageMemoryLayout
{
    private readonly long _x;
    private readonly long _y;

    public XyzImageMemoryLayout(ImageShape shape) : base(shape)
    {
        _x = 1L * Shape.Height * Shape.Channels;
        _y = 1L * Shape.Channels;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override long Flatten(long x, long y, int z) => x * _x + y * _y + z;
}