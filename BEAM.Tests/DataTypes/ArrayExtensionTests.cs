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