namespace BEAM.Tests.DataTypes;

using Datatypes;
using Xunit;

public class RectangleTests
{
    [Fact]
    public void Constructor_SetsCorrectValues()
    {
        var topLeft = new Coordinate2D(1, 2);
        var bottomRight = new Coordinate2D(3, 4);
        var rectangle = new Rectangle(topLeft, bottomRight);

        Assert.Equal(topLeft, rectangle.TopLeft);
        Assert.Equal(bottomRight, rectangle.BottomRight);
    }

    [Fact]
    public void Contains_ReturnsTrue_WhenCoordinateIsInsideRectangle()
    {
        var topLeft = new Coordinate2D(1, 1);
        var bottomRight = new Coordinate2D(3, 3);
        var rectangle = new Rectangle(topLeft, bottomRight);
        var coordinate = new Coordinate2D(2, 2);

        var result = rectangle.Contains(coordinate);

        Assert.True(result);
    }

    [Fact]
    public void Contains_ReturnsFalse_WhenCoordinateIsOutsideRectangle()
    {
        var topLeft = new Coordinate2D(1, 1);
        var bottomRight = new Coordinate2D(3, 3);
        var rectangle = new Rectangle(topLeft, bottomRight);
        var coordinate = new Coordinate2D(4, 4);

        var result = rectangle.Contains(coordinate);

        Assert.False(result);
    }

    [Fact]
    public void Contains_ReturnsFalse_WhenCoordinateIsOnTopLeftBoundary()
    {
        var topLeft = new Coordinate2D(1, 1);
        var bottomRight = new Coordinate2D(3, 3);
        var rectangle = new Rectangle(topLeft, bottomRight);
        var coordinate = new Coordinate2D(1, 1);

        var result = rectangle.Contains(coordinate);

        Assert.True(result);
    }

    [Fact]
    public void Contains_ReturnsFalse_WhenCoordinateIsOnBottomRightBoundary()
    {
        var topLeft = new Coordinate2D(1, 1);
        var bottomRight = new Coordinate2D(3, 3);
        var rectangle = new Rectangle(topLeft, bottomRight);
        var coordinate = new Coordinate2D(3, 3);

        var result = rectangle.Contains(coordinate);

        Assert.True(result);
    }
}