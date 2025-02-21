using BEAM.Converter;

namespace BEAM.Tests;

public class ConverterTests
{
    [Fact]
    public void TestLogLevelIconConverter()
    {
        var converter = new LogLevelIconConverter();
        Assert.Equal("../Assets/info.svg", converter.Convert("INFO", null, null, null));
        Assert.Equal("../Assets/warning.svg", converter.Convert("WARNING", null, null, null));
        Assert.Equal("../Assets/error.svg", converter.Convert("ERROR", null, null, null));
        Assert.Equal(string.Empty, converter.Convert("lorem ipsum", null, null, null));
    }

    [Fact]
    public void TestLengthToMaxConverter()
    {
        var converter = new LengthToMaxConverter();
        Assert.Equal(24, converter.Convert("25", null, null, null));
        Assert.Equal(0, converter.Convert("1", null, null, null));
        Assert.Equal(99, converter.Convert("100", null, null, null));
        Assert.Equal(int.MaxValue - 1, converter.Convert(int.MaxValue, null, null, null));
    }
}