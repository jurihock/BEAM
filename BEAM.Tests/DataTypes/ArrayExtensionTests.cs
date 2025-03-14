namespace BEAM.Tests.DataTypes;

using BEAM.Datatypes;
using Xunit;

public class ArrayExtensionsTests
{
    [Fact]
    public void ArgMax_ReturnsCorrectIndex_WhenArrayHasPositiveValues()
    {
        var values = new double[] { 1.0, 3.0, 2.0 };

        var result = values.ArgMax();

        Assert.Equal(1, result);
    }

    [Fact]
    public void ArgMax_ReturnsCorrectIndex_WhenArrayHasNegativeValues()
    {
        var values = new double[] { -1.0, -3.0, -2.0 };

        var result = values.ArgMax();

        Assert.Equal(0, result);
    }

    [Fact]
    public void ArgMax_ReturnsCorrectIndex_WhenArrayHasMixedValues()
    {
        var values = new double[] { -1.0, 3.0, -2.0 };

        var result = values.ArgMax();

        Assert.Equal(1, result);
    }

    [Fact]
    public void ArgMax_ReturnsCorrectIndex_WhenNANValues()
    {
        var values = new double[] { double.NaN, 3.0, -2.0 };

        var result = values.ArgMax();

        Assert.Equal(1, result);
    }

    [Fact]
    public void ArgMax_ReturnsCorrectIndex_WhenPositiveInfinityValues()
    {
        var values = new double[] { double.PositiveInfinity, 3.0, -2.0 };

        var result = values.ArgMax();

        Assert.Equal(0, result);
    }

    [Fact]
    public void ArgMax_ReturnsCorrectIndex_WhenNegativeInfinityValues()
    {
        var values = new double[] { double.NegativeInfinity, 3.0, -2.0 };

        var result = values.ArgMax();

        Assert.Equal(1, result);
    }

    [Fact]
    public void ArgMax_ReturnsFirstIndex_WhenArrayHasMultipleMaxValues()
    {
        var values = new double[] { 1.0, 3.0, 3.0, 2.0 };

        var result = values.ArgMax();

        Assert.Equal(1, result);
    }

    [Fact]
    public void ArgMax_ReturnsFirstIndex_WhenArrayLarge()
    {
        var values = new double[]
        {
            13, 16, 38, 46, 71, 96, 20, 47, 95, 80, 36, 74, 31, 61, 29, 73, 63, 100, 78, 27, 52, 6, 37, 67, 85, 56, 4,
            57, 68, 62, 19, 93, 50, 59, 88, 65, 81, 44, 11, 99, 18, 39, 21, 75, 70, 72, 35, 97, 15, 66, 58, 1, 92, 41,
            32, 79, 51, 34, 83, 3, 14, 98, 48, 40, 49, 89, 5, 86, 25, 9, 90, 17, 84, 87, 8, 94, 30, 54, 28, 77, 53, 43,
            23, 10, 33, 91, 22, 76, 12, 55, 69, 24, 82, 26, 2, 7, 60, 42, 64, 45
        };

        var result = values.ArgMax();

        Assert.Equal(17, result);
    }

    [Fact]
    public void ArgMax_ThrowsArgumentException_WhenArrayIsEmpty()
    {
        var values = new double[] { };

        Assert.Throws<ArgumentException>(() => values.ArgMax());
    }

    [Fact]
    public void ArgMax_ReturnsZero_WhenArrayHasSingleElement()
    {
        var values = new double[] { 5.0 };

        var result = values.ArgMax();

        Assert.Equal(0, result);
    }
}