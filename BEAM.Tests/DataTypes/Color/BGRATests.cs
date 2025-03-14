using BEAM.Datatypes.Color;

namespace BEAM.Tests.DataTypes.Color;

public class BGRATests
{
    [Fact]
    public void Constructor_SetsCorrectValues()
    {
        var baseColor = new BGR(10, 20, 30);
        var color = new BGRA(baseColor, 40);

        Assert.Equal(10, color.B);
        Assert.Equal(20, color.G);
        Assert.Equal(30, color.R);
        Assert.Equal(40, color.A);
    }

    [Fact]
    public void Indexer_Get_ReturnsCorrectValue()
    {
        var baseColor = new BGR(10, 20, 30);
        var color = new BGRA(baseColor, 40);

        Assert.Equal(10, color[0]);
        Assert.Equal(20, color[1]);
        Assert.Equal(30, color[2]);
        Assert.Equal(40, color[3]);
    }

    [Fact]
    public void Indexer_Set_SetsCorrectValue()
    {
        var baseColor = new BGR(10, 20, 30);
        var color = new BGRA(baseColor, 40);

        color[0] = 50;
        color[1] = 60;
        color[2] = 70;
        color[3] = 80;

        Assert.Equal(50, color.B);
        Assert.Equal(60, color.G);
        Assert.Equal(70, color.R);
        Assert.Equal(80, color.A);
    }

    [Fact]
    public void Indexer_ThrowsArgumentOutOfRangeException_WhenIndexIsInvalid()
    {
        var baseColor = new BGR(10, 20, 30);
        var color = new BGRA(baseColor, 40);

        Assert.Throws<ArgumentOutOfRangeException>(() => color[-1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => color[4]);
    }
}