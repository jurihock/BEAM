using BEAM.ImageSequence;

namespace BEAM.Tests.ImageSequence;
using Xunit;
[Collection("GlobalTests")]
public class SequenceShapeTests
{
    [Fact]
    public void Equals_ReturnsTrue_ForIdenticalShapes()
    {
        var shape1 = new SequenceShape(1920, 1080, 3);
        var shape2 = new SequenceShape(1920, 1080, 3);

        Assert.True(shape1.Equals(shape2));
    }

    [Fact]
    public void Equals_ReturnsFalse_ForDifferentShapes()
    {
        var shape1 = new SequenceShape(1920, 1080, 3);
        var shape2 = new SequenceShape(1280, 720, 3);

        Assert.False(shape1.Equals(shape2));
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenOtherIsNull()
    {
        var shape = new SequenceShape(1920, 1080, 3);

        Assert.False(shape.Equals(null));
    }

    [Fact]
    public void GetHashCode_ReturnsSameHashCode_ForIdenticalShapes()
    {
        var shape1 = new SequenceShape(1920, 1080, 3);
        var shape2 = new SequenceShape(1920, 1080, 3);

        Assert.Equal(shape1.GetHashCode(), shape2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ReturnsDifferentHashCode_ForDifferentShapes()
    {
        var shape1 = new SequenceShape(1920, 1080, 3);
        var shape2 = new SequenceShape(1280, 720, 3);

        Assert.NotEqual(shape1.GetHashCode(), shape2.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        var shape = new SequenceShape(1920, 1080, 3);

        Assert.Equal("1920x1080x3", shape.ToString());
    }

    [Fact]
    public void OperatorEquals_ReturnsTrue_ForIdenticalShapes()
    {
        var shape1 = new SequenceShape(1920, 1080, 3);
        var shape2 = new SequenceShape(1920, 1080, 3);

        Assert.True(shape1 == shape2);
    }

    [Fact]
    public void OperatorEquals_ReturnsFalse_ForDifferentShapes()
    {
        var shape1 = new SequenceShape(1920, 1080, 3);
        var shape2 = new SequenceShape(1280, 720, 3);

        Assert.False(shape1 == shape2);
    }

    [Fact]
    public void OperatorNotEquals_ReturnsTrue_ForDifferentShapes()
    {
        var shape1 = new SequenceShape(1920, 1080, 3);
        var shape2 = new SequenceShape(1280, 720, 3);

        Assert.True(shape1 != shape2);
    }

    [Fact]
    public void OperatorNotEquals_ReturnsFalse_ForIdenticalShapes()
    {
        var shape1 = new SequenceShape(1920, 1080, 3);
        var shape2 = new SequenceShape(1920, 1080, 3);

        Assert.False(shape1 != shape2);
    }
}