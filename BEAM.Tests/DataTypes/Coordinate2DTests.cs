using ScottPlot;

namespace BEAM.Tests.DataTypes;

using BEAM.Datatypes;
using Xunit;

public class Coordinate2DTests
{
    [Fact]
    public void Constructor_SetsCorrectValues_WithIntParameters()
    {
        var coordinate = new Coordinate2D(1, 2);

        Assert.Equal(1, coordinate.Row);
        Assert.Equal(2, coordinate.Column);
    }

    [Fact]
    public void Constructor_SetsCorrectValues_WithLongParameters()
    {
        var coordinate = new Coordinate2D(1L, 2L);

        Assert.Equal(1L, coordinate.Row);
        Assert.Equal(2L, coordinate.Column);
    }

    [Fact]
    public void Constructor_SetsCorrectValues_WithDoubleParameters()
    {
        var coordinate = new Coordinate2D(1.5, 2.5);

        Assert.Equal(1.5, coordinate.Row);
        Assert.Equal(2.5, coordinate.Column);
    }

    [Fact]
    public void Constructor_SetsCorrectValues_WithCoordinatesParameter()
    {
        var coordinates = new Coordinates(2.5, 1.5);
        var coordinate = new Coordinate2D(coordinates);

        Assert.Equal(1.5, coordinate.Row);
        Assert.Equal(2.5, coordinate.Column);
    }

    [Fact]
    public void OffsetBy_ReturnsCorrectOffsetCoordinate()
    {
        var coordinate = new Coordinate2D(1.0, 2.0);
        var offsetCoordinate = coordinate.OffsetBy(1.0, 1.0);

        Assert.Equal(2.0, offsetCoordinate.Row);
        Assert.Equal(3.0, offsetCoordinate.Column);
    }

    [Fact]
    public void ToString_ReturnsCorrectStringRepresentation()
    {
        var coordinate = new Coordinate2D(1.0, 2.0);

        var result = coordinate.ToString();

        Assert.Equal("Row: 1, Column: 2", result);
    }

    [Fact]
    public void Equals_ReturnsTrue_ForEqualCoordinates()
    {
        var coordinate1 = new Coordinate2D(1.0, 2.0);
        var coordinate2 = new Coordinate2D(1.0, 2.0);

        var result = coordinate1.Equals(coordinate2);

        Assert.True(result);
    }

    [Fact]
    public void Equals_ReturnsFalse_ForDifferentCoordinates()
    {
        var coordinate1 = new Coordinate2D(1.0, 2.0);
        var coordinate2 = new Coordinate2D(2.0, 1.0);

        var result = coordinate1.Equals(coordinate2);

        Assert.False(result);
    }

    [Fact]
    public void GetHashCode_ReturnsSameHashCode_ForEqualCoordinates()
    {
        var coordinate1 = new Coordinate2D(1.0, 2.0);
        var coordinate2 = new Coordinate2D(1.0, 2.0);

        var hashCode1 = coordinate1.GetHashCode();
        var hashCode2 = coordinate2.GetHashCode();

        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void GetHashCode_ReturnsDifferentHashCode_ForDifferentCoordinates()
    {
        var coordinate1 = new Coordinate2D(1.0, 2.0);
        var coordinate2 = new Coordinate2D(2.0, 1.0);

        var hashCode1 = coordinate1.GetHashCode();
        var hashCode2 = coordinate2.GetHashCode();

        Assert.NotEqual(hashCode1, hashCode2);
    }
}