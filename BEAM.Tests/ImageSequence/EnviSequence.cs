using System.Buffers;
using System.Runtime.CompilerServices;
using BEAM.Exceptions;
using BEAM.Image;
using BEAM.ImageSequence;
using BEAM.Models.Log;
using Xunit.Abstractions;

namespace BEAM.Tests.ImageSequence;

[Collection("GlobalTests")]
public class EnviSequenceTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public EnviSequenceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void SingleImageHeight_ReturnsCorrectHeight()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/EnviTests/EnviTest") };

        var sequence = new EnviSequence(list, "CoolSequence");

        Assert.Equal(600, sequence.SingleImageHeight);
    }

    [Fact]
    public void Shape_ReturnsCorrectShape()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/EnviTests/EnviTest") };

        var sequence = new EnviSequence(list, "CoolSequence");

        Assert.Equal(new ImageShape(800, 600, 4), sequence.Shape);
    }
    
    [Fact]
    public void GetPixelOnlyCoordinates_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/EnviTests/EnviTest") };

        var sequence = new EnviSequence(list, "CoolSequence");

        Assert.Equal(new double[] {0, 0, 0, 0}, sequence.GetPixel(0, 0));
        Assert.Equal(new double[] {55, 55, 159, 205}, sequence.GetPixel(448, 228));
    }

    [Fact]
    public void GetPixelChannel_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/EnviTests/EnviTest") };

        var sequence = new EnviSequence(list, "CoolSequence");
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
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/EnviTests/EnviTest") };

        var sequence = new EnviSequence(list, "CoolSequence");
        Assert.Equal(new double[] {0, 0, 0, 0}, sequence.GetPixel(0, 0, [0, 1, 2, 3]));
        Assert.Equal(new double[] {55, 159, 205}, sequence.GetPixel(448, 228, [0, 2, 3]));
        Assert.Equal(new double[] {55, 55}, sequence.GetPixel(448, 228, [0, 1]));
    }

    [Fact]
    public void GetPixelLineData_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/EnviTests/EnviTest") };

        var sequence = new EnviSequence(list, "CoolSequence");
        var buffer = ArrayPool<double>.Shared;
        var lineData = sequence.GetPixelLineData(0, [0, 1, 2, 3]);
        
        Assert.Equal(new ImageShape(800, 1, 4), lineData.Shape);
        Assert.Equal([0, 0, 0, 0], lineData.GetPixel(0, 0));
        Assert.Equal(0, lineData.GetPixel(2, 0, 1));
    }

    [Fact]
    public void GetPixelLineDataXs_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/EnviTests/EnviTest") };

        var sequence = new EnviSequence(list, "CoolSequence");
        var buffer = ArrayPool<double>.Shared;
        var xs = new long[] {0, 1, 2, 3, 500, 798, 799};
        
        var lineData = sequence.GetPixelLineData(xs, 0, [0, 1, 2, 3]);
        Assert.Equal(new ImageShape(7, 1, 4), lineData.Shape);
        Assert.Equal([0, 0, 0, 0], lineData.GetPixel(0, 0));
        Assert.Equal(0, lineData.GetPixel(2, 0, 1));
        Assert.Equal(0, lineData.GetPixel(4, 0, 3));
        Assert.Equal(0, lineData.GetPixel(6, 0, 2));
    }

    [Fact]
    public void GetName_ReturnsCorrectName()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/EnviTests/EnviTest") };

        var sequence = new EnviSequence(list, "CoolSequence");
        Assert.Equal("CoolSequence", sequence.GetName());
        
        var sequence2 = new EnviSequence(list, "BeamSuperDuperSequence!");
        Assert.Equal("BeamSuperDuperSequence!", sequence2.GetName());
    }
    
    [Fact]
    public void GetLoadedImageCount_ReturnsCorrectCount()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/EnviTests/EnviTest") };

        var sequence = new EnviSequence(list, "CoolSequence");
        Assert.Equal(1, sequence.GetLoadedImageCount());
    }

    [Fact]
    public void GetImage_ReturnsCorrectImage()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/EnviTests/EnviTest") };
        
        var sequence = new EnviSequence(list, "CoolSequence");
        Assert.Throws<ArgumentOutOfRangeException>(() => sequence.GetImage(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => sequence.GetImage(2));
        
        Assert.Equal(new ImageShape(800, 600, 4), sequence.GetImage(0).Shape);
        
        Assert.Equal([0, 0, 0, 0], sequence.GetImage(0).GetPixel(0, 0));
        
        Assert.Equal([0, 0, 0, 0], sequence.GetImage(0).GetPixel(560, 471));
    }

    [Fact]
    public void OpenEnviSequence_HandlesEmptyPath()
    {
        Assert.Throws<EmptySequenceException>(() => EnviSequence.Open(new List<string>(), "TestName"));
    }
    
    [Fact]
    public void OpenEnviSequence_HandlesNoCorrectSequence()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/NoSequence/Test.txt") };
        Assert.Throws<UnknownSequenceException>(() => EnviSequence.Open(list, "Test1234"));
    }
    
    [Fact]
    public void OpenEnviSequenceFolder_HandlesEmptyFolder()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var str = Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/NoSequence/");
        Assert.Throws<UnknownSequenceException>(() => EnviSequence.Open(str));
        Assert.Throws<UnknownSequenceException>(() => EnviSequence.Open(""));
    }
    
    // Helper method to get the path of the current file at compile time
    private static string GetFilePath([CallerFilePath] string filePath = "")
    {
        return filePath;
    }
}