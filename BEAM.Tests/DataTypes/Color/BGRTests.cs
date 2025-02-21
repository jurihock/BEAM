namespace BEAM.Tests.DataTypes;

using BEAM.Datatypes.Color;
using Xunit;

public class BGRTests
{
    [Fact]
    public void Constructor_SetsCorrectValues()
    {
        var color = new BGR(10, 20, 30);

        Assert.Equal(10, color.B);
        Assert.Equal(20, color.G);
        Assert.Equal(30, color.R);
    }
    
    [Fact]
    public void Constructor_SetsCorrectValuesZero()
    {
        var color = new BGR(0, 0, 0);

        Assert.Equal(0, color.B);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.R);
    }
    
    [Fact]
    public void Constructor_SetsCorrectValuesMax()
    {
        var color = new BGR(255, 255, 255);

        Assert.Equal(255, color.B);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.R);
    }

    [Fact]
    public void Indexer_Get_ReturnsCorrectValue()
    {
        var color = new BGR(10, 20, 30);

        Assert.Equal(10, color[0]);
        Assert.Equal(20, color[1]);
        Assert.Equal(30, color[2]);
    }
    
    [Fact]
    public void Indexer_Set_SetsCorrectValue()
    {
        var color = new BGR(10, 20, 30);

        color[0] = 40;
        color[1] = 50;
        color[2] = 60;

        Assert.Equal(40, color.B);
        Assert.Equal(50, color.G);
        Assert.Equal(60, color.R);
    }

    [Fact]
    public void Indexer_ThrowsArgumentOutOfRangeException_WhenIndexIsInvalid()
    {
        var color = new BGR(10, 20, 30);

        Assert.Throws<ArgumentOutOfRangeException>(() => color[-1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => color[3]);
    }

    [Fact]
    public void Constructor_FromArray_SetsCorrectValues()
    {
        var color = new BGR(new byte[] { 10, 20, 30 });

        Assert.Equal(10, color.B);
        Assert.Equal(20, color.G);
        Assert.Equal(30, color.R);
    }

    [Fact]
    public void Constructor_FromArray_ThrowsArgumentException_WhenArrayLengthIsInvalid()
    {
        Assert.Throws<IndexOutOfRangeException>(() => new BGR(new byte[] { 10, 20 }));
        Assert.Throws<ArgumentException>(() => new BGR(new byte[] { 10, 20, 30, 40 }));
    }
}