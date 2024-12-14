using BEAM.ImageSequence;

namespace BEAM.Tests.ImageSequence;

public class SequenceTest
{
    [Fact]
    public void OpenCorrectSequenceType()
    {
        // Checks for supported types
        Assert.IsType<SkiaSequence>(Sequence.Open(["1.png", "2.png", "3.png", "4.txt", "5.txt"]));
        Assert.IsType<EnviSequence>(Sequence.Open(["1.raw", "2.hdr", "3.abc", "4.txt", "5.txt"]));

        // Check for order
        Assert.IsType<SkiaSequence>(Sequence.Open(["1.png", "1.hdr", "1.raw"]));
        Assert.IsType<EnviSequence>(Sequence.Open(["1.raw", "0.png", "1.hdr"]));
        Assert.IsType<EnviSequence>(Sequence.Open(["1.hdr", ".png", "1.raw"]));

        // Checks if exception is throws on unsupported exception
        Assert.ThrowsAny<Exception>(() => Sequence.Open(["1.abc", "2.def"]));
    }
}