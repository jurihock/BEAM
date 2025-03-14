using System.Buffers;
using System.Runtime.CompilerServices;
using BEAM.Image;
using BEAM.ImageSequence;
using Xunit.Abstractions;

namespace BEAM.Tests.ImageSequence;

public class SkiaSequenceTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public SkiaSequenceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void SingleImageHeight_ReturnsCorrectHeight()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");

        Assert.Equal(600, sequence.SingleImageHeight);
    }

    [Fact]
    public void Shape_ReturnsCorrectShape()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");

        Assert.Equal(new ImageShape(800, 600, 4), sequence.Shape);
    }
    
    [Fact]
    public void GetPixelOnlyCoordinates_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");

        Assert.Equal(new double[] {0, 0, 0, 0}, sequence.GetPixel(0, 0));
        Assert.Equal(new double[] {55, 55, 159, 205}, sequence.GetPixel(448, 228));
    }

    [Fact]
    public void GetPixelChannel_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        Assert.Equal(205 , sequence.GetPixel(448, 228 , 3));
        Assert.Equal(0, sequence.GetPixel(2, 2 , 1));
        Assert.Equal(128, sequence.GetPixel(455, 453 , 2));
        Assert.Equal(128, sequence.GetPixel(455, 453 , 1));
        Assert.Equal(44, sequence.GetPixel(455, 453 , 0));
    }
    
    [Fact]
    public void GetPixelChannels_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        Assert.Equal(new double[] {0, 0, 0, 0}, sequence.GetPixel(0, 0, [0, 1, 2, 3]));
        Assert.Equal(new double[] {55, 159, 205}, sequence.GetPixel(448, 228, [0, 2, 3]));
        Assert.Equal(new double[] {55, 55}, sequence.GetPixel(448, 228, [0, 1]));
    }

    [Fact]
    public void GetPixelLineData_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var buffer = ArrayPool<double>.Shared;
        var lineData = sequence.GetPixelLineData(0, [0, 1, 2, 3], buffer);
        
        Assert.Equal(new ImageShape(800, 1, 16), lineData.Shape);
        Assert.Equal([0, 0, 0, 0], lineData.GetPixel(0, 0));
        Assert.Equal(0, lineData.GetPixel(2, 0, 1));
    }
    
    // Helper method to get the path of the current file at compile time
    private static string GetFilePath([CallerFilePath] string filePath = "")
    {
        return filePath;
    }
}