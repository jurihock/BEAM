using BEAM.Image;

namespace BEAM.Tests.Image;

using Xunit;
[Collection("GlobalTests")]
public class LineImageTests
{
    [Fact]
    public void Constructor_ThrowsException_WhenDataIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new LineImage(new double[][] { }));
    }

    [Fact]
    public void Constructor_ThrowsException_WhenDataIsNull()
    {
        Assert.Throws<ArgumentException>(() => new LineImage(null));
    }

    [Fact]
    public void Constructor_ThrowsException_WhenDataHasEmptyRow()
    {
        Assert.Throws<ArgumentException>(() => new LineImage(new double[][] { new double[] { } }));
    }

    [Fact]
    public void GetPixel_ReturnsCorrectValue_ForValidCoordinates()
    {
        var data = new double[][] { [1.0, 2.0, 3.0] };
        var lineImage = new LineImage(data);

        var result = lineImage.GetPixel(0, 0, 1);

        Assert.Equal(2.0, result);
    }

    [Fact]
    public void GetPixel_ThrowsException_ForInvalidYCoordinate()
    {
        var data = new double[][] { [1.0, 2.0, 3.0] };
        var lineImage = new LineImage(data);

        Assert.Throws<ArgumentOutOfRangeException>(() => lineImage.GetPixel(0, 1, 1));
    }

    [Fact]
    public void GetPixel_ThrowsException_ForInvalidXCoordinate()
    {
        var data = new double[][] { [1.0, 2.0, 3.0] };
        var lineImage = new LineImage(data);

        Assert.Throws<ArgumentOutOfRangeException>(() => lineImage.GetPixel(-1, 0, 1));
    }

    [Fact]
    public void GetPixel_ThrowsException_ForInvalidChannel()
    {
        var data = new double[][] { [1.0, 2.0, 3.0] };
        var lineImage = new LineImage(data);

        Assert.Throws<ArgumentOutOfRangeException>(() => lineImage.GetPixel(0, 0, 3));
    }

    [Fact]
    public void GetPixelArray_ReturnsCorrectValues_ForValidCoordinates()
    {
        var data = new double[][] { [1.0, 2.0, 3.0] };
        var lineImage = new LineImage(data);

        var result = lineImage.GetPixel(0, 0);

        Assert.Equal(ExpectedArray, result);
    }

    [Fact]
    public void GetPixelArray_ThrowsException_ForInvalidYCoordinate()
    {
        var data = new double[][] { [1.0, 2.0, 3.0] };
        var lineImage = new LineImage(data);

        Assert.Throws<ArgumentOutOfRangeException>(() => lineImage.GetPixel(0, 1));
    }

    [Fact]
    public void GetPixelArray_ThrowsException_ForInvalidXCoordinate()
    {
        var data = new double[][] { [1.0, 2.0, 3.0] };
        var lineImage = new LineImage(data);

        Assert.Throws<ArgumentOutOfRangeException>(() => lineImage.GetPixel(-1, 0));
    }

    private static readonly double[] Expected = [1.0, 3.0];
    private static readonly double[] ExpectedArray = [1.0, 2.0, 3.0];

    [Fact]
    public void GetPixelWithChannels_ReturnsCorrectValues_ForValidCoordinatesAndChannels()
    {
        var data = new double[][] { [1.0, 2.0, 3.0] };
        var lineImage = new LineImage(data);

        var result = lineImage.GetPixel(0, 0, [0, 2]);

        Assert.Equal(Expected, result);
    }

    [Fact]
    public void GetPixelWithChannels_ThrowsException_ForInvalidYCoordinate()
    {
        var data = new double[][] { [1.0, 2.0, 3.0] };
        var lineImage = new LineImage(data);

        Assert.Throws<ArgumentOutOfRangeException>(() => lineImage.GetPixel(0, 1, [0, 2]));
    }

    [Fact]
    public void GetPixelWithChannels_ThrowsException_ForInvalidXCoordinate()
    {
        var data = new double[][] { [1.0, 2.0, 3.0] };
        var lineImage = new LineImage(data);

        Assert.Throws<ArgumentOutOfRangeException>(() => lineImage.GetPixel(-1, 0, [0, 2]));
    }

    [Fact]
    public void GetPixelWithChannels_ThrowsException_ForInvalidChannel()
    {
        var data = new double[][] { [1.0, 2.0, 3.0] };
        var lineImage = new LineImage(data);

        Assert.Throws<ArgumentOutOfRangeException>(() => lineImage.GetPixel(0, 0, [0, 3]));
    }
}