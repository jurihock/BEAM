using System.Buffers;
using System.Runtime.CompilerServices;
using BEAM.Exceptions;
using BEAM.Image;
using BEAM.ImageSequence;
using BEAM.Models.Log;
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
        var lineData = sequence.GetPixelLineData(0, [0, 1, 2, 3]);
        
        Assert.Equal(new ImageShape(800, 1, 4), lineData.Shape);
        Assert.Equal([0, 0, 0, 0], lineData.GetPixel(0, 0));
        Assert.Equal(0, lineData.GetPixel(2, 0, 1));
    }

    [Fact]
    public void GetPixelLineDataXs_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
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
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        Assert.Equal("CoolSequence", sequence.GetName());
        
        var sequence2 = new SkiaSequence(list, "BeamSuperDuperSequence!");
        Assert.Equal("BeamSuperDuperSequence!", sequence2.GetName());
    }
    
    [Fact]
    public void GetLoadedImageCount_ReturnsCorrectCount()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        Assert.Equal(1, sequence.GetLoadedImageCount());
        
        list.Add(Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Flag_of_Deutschland.png"));
        var sequence2 = new SkiaSequence(list, "BeamSuperDuperSequence!");
        Assert.Equal(2, sequence2.GetLoadedImageCount());
    }

    [Fact]
    public void GetImage_ReturnsCorrectImage()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };
        list.Add(Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Flag_of_Deutschland.png"));
        
        var sequence = new SkiaSequence(list, "CoolSequence");
        Assert.Throws<ArgumentOutOfRangeException>(() => sequence.GetImage(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => sequence.GetImage(2));
        
        Assert.Equal(new ImageShape(800, 600, 4), sequence.GetImage(0).Shape);
        Assert.Equal(new ImageShape(800, 600, 4), sequence.GetImage(1).Shape);
        
        Assert.Equal([0, 0, 0, 0], sequence.GetImage(0).GetPixel(0, 0));
        Assert.Equal([0, 0, 0, 0], sequence.GetImage(1).GetPixel(0, 0));
        
        Assert.Equal([0, 0, 0, 0], sequence.GetImage(0).GetPixel(560, 471));
        Assert.Equal([0, 206, 255, 255], sequence.GetImage(1).GetPixel(560, 471));
        
        Assert.False(sequence.GetImage(0) == sequence.GetImage(1));
    }

    [Fact]
    public void OpenSkiaSequence_HandlesEmptyPath()
    {
        Assert.Throws<EmptySequenceException>(() => SkiaSequence.Open(new List<string>(), "TestName"));
    }
    
    [Fact]
    public void OpenSkiaSequence_HandlesNoCorrectSequence()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/NoSequence/Test.txt") };
        Assert.Throws<UnknownSequenceException>(() => SkiaSequence.Open(list, "Test1234"));
    }

    [Fact]
    public void OpenSkiaSequence_IgnoresInvalidShapes()
    {
        Logger.Init();
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };
        list.Add(Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Flag_of_Europe.png"));

        var sequence = SkiaSequence.Open(list, "Sun5on");
        Assert.Equal(1, sequence.GetLoadedImageCount());
    }

    [Fact]
    public void OpenSkiaSequenceFolder_IgnoresInvalidFiles()
    {
        Logger.Init();
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var str = Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets");
        
        var sequence = SkiaSequence.Open(str);
        path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var name = Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "..");
        Assert.Equal(name, sequence.GetName());
        //Finds Germany and ignores the EU due to invalid shape and then finds Test.png
        Assert.Equal(2, sequence.GetLoadedImageCount());
        Assert.Equal(new ImageShape(800, 1200, 4), sequence.Shape);
    }
    
    [Fact]
    public void OpenSkiaSequenceFolder_HandlesEmptyFolder()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var str = Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/NoSequence/");
        Assert.Throws<UnknownSequenceException>(() => SkiaSequence.Open(str));
        Assert.Throws<UnknownSequenceException>(() => SkiaSequence.Open(""));
    }
    
    // Helper method to get the path of the current file at compile time
    private static string GetFilePath([CallerFilePath] string filePath = "")
    {
        return filePath;
    }
}