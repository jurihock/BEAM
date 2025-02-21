namespace BEAM.Tests.Image;

using BEAM.Image;
using Xunit;

public class ImageShapeTests
{
    [Fact]
    public void Constructor_SetsCorrectValues()
    {
        var shape = new ImageShape(1920, 1080, 3);

        Assert.Equal(1920, shape.Width);
        Assert.Equal(1080, shape.Height);
        Assert.Equal(3, shape.Channels);
        Assert.Equal(1920 * 1080, shape.Area);
        Assert.Equal(1920 * 1080 * 3, shape.Volume);
    }

    [Fact]
    public void CopyConstructor_CopiesValuesCorrectly()
    {
        var original = new ImageShape(1920, 1080, 3);
        var copy = new ImageShape(original);

        Assert.Equal(original.Width, copy.Width);
        Assert.Equal(original.Height, copy.Height);
        Assert.Equal(original.Channels, copy.Channels);
        Assert.Equal(original.Area, copy.Area);
        Assert.Equal(original.Volume, copy.Volume);
    }

    [Fact]
    public void Equals_ReturnsTrue_ForEqualShapes()
    {
        var shape1 = new ImageShape(1920, 1080, 3);
        var shape2 = new ImageShape(1920, 1080, 3);

        Assert.True(shape1.Equals(shape2));
        Assert.True(shape1 == shape2);
    }

    [Fact]
    public void Equals_ReturnsFalse_ForDifferentShapes()
    {
        var shape1 = new ImageShape(1920, 1080, 3);
        var shape2 = new ImageShape(1280, 720, 3);

        Assert.False(shape1.Equals(shape2));
        Assert.False(shape1 == shape2);
    }

    [Fact]
    public void NotEqualsOperator_ReturnsTrue_ForDifferentShapes()
    {
        var shape1 = new ImageShape(1920, 1080, 3);
        var shape2 = new ImageShape(1280, 720, 3);

        Assert.True(shape1 != shape2);
    }

    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        var shape = new ImageShape(1920, 1080, 3);
        var expected = "1920x1080x3";

        Assert.Equal(expected, shape.ToString());
    }

    [Fact]
    public void GetHashCode_ReturnsSameHashCode_ForEqualShapes()
    {
        var shape1 = new ImageShape(1920, 1080, 3);
        var shape2 = new ImageShape(1920, 1080, 3);

        Assert.Equal(shape1.GetHashCode(), shape2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ReturnsDifferentHashCode_ForDifferentShapes()
    {
        var shape1 = new ImageShape(1920, 1080, 3);
        var shape2 = new ImageShape(1280, 720, 3);

        Assert.NotEqual(shape1.GetHashCode(), shape2.GetHashCode());
    }
}