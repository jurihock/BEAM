using System.Runtime.CompilerServices;

namespace BEAM.Image;

public sealed class YxzImageMemoryLayout : ImageMemoryLayout
{
    private readonly long X;
    private readonly long Y;

    public YxzImageMemoryLayout(ImageShape shape) : base(shape)
    {
        X = 1L * Shape.Channels;
        Y = 1L * Shape.Width * Shape.Channels;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override long Flatten(long x, long y, int z) => x * X + y * Y + z;
}