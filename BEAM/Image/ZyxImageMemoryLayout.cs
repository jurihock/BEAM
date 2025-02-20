using System.Runtime.CompilerServices;

namespace BEAM.Image;

public sealed class ZyxImageMemoryLayout : ImageMemoryLayout
{
    private readonly long _y;
    private readonly long _z;

    public ZyxImageMemoryLayout(ImageShape shape) : base(shape)
    {
        _y = 1L * Shape.Width;
        _z = 1L * Shape.Height * Shape.Width;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override long Flatten(long x, long y, int z) => x + y * _y + z * _z;
}