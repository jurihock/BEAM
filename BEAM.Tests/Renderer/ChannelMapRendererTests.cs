using System.Runtime.CompilerServices;
using BEAM.Datatypes.Color;
using BEAM.ImageSequence;
using BEAM.Renderer;
using Xunit.Abstractions;

namespace BEAM.Tests.Renderer;

public class ChannelMapRendererTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ChannelMapRendererTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void RenderPixel_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var renderer = new ChannelMapRenderer(0, 255, 2, 1, 0);
        
        var result = renderer.RenderPixel(sequence, 415, 233);
        Assert.Equal(new BGR(55, 55, 161), result);
        
        result = renderer.RenderPixel(sequence, 480, 260);
        Assert.Equal(new BGR(255, 255, 255), result);
    }

    [Fact]
    public void RenderPixels_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var renderer = new ChannelMapRenderer(0, 255, 2, 1, 0);
        
        BGR[] output = [new BGR(0, 0, 0), new BGR(0, 0, 0)];
        renderer.RenderPixels(sequence, [0, 415], 233, output);
        Assert.Equal(new BGR(55, 55, 161), output[1]);
        Assert.Equal(new BGR(0, 0, 0), output[0]);
    }
    
    [Fact]
    public void CorrectChannelChangeRenderPixels()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var renderer = new ChannelMapRenderer(0, 255, 2, 1, 0);
        
        var result = renderer.RenderPixel(sequence, 415, 233);
        Assert.Equal(new BGR(55, 55, 161), result);
        
        result = renderer.RenderPixel(sequence, 480, 260);
        Assert.Equal(new BGR(255, 255, 255), result);
        
        renderer.ChannelBlue = 2;
        renderer.ChannelRed = 0;
        result = renderer.RenderPixel(sequence, 415, 233);
        Assert.Equal(new BGR(161, 55, 55), result);
        
        renderer.ChannelBlue = 1;
        renderer.ChannelGreen = 2;
        result = renderer.RenderPixel(sequence, 415, 233);
        Assert.Equal(new BGR(55, 161, 55), result);
    }

    [Fact]
    public void CorrectDifferentChannelsRenderPixels()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var renderer = new ChannelMapRenderer(0, 255, 0, 2, 1);
        
        BGR[] output = [new BGR(0, 0, 0), new BGR(0, 0, 0)];
        renderer.RenderPixels(sequence, [0, 415], 233, output);
        Assert.Equal(new BGR(55, 161, 55), output[1]);
        Assert.Equal(new BGR(0, 0, 0), output[0]);
    }
    
    [Fact]
    public void ChannelMapRenderer_ReturnsCorrectName()
    {
        Assert.Equal("Channel Map", new ChannelMapRenderer(0, 255, 2, 1, 0).GetName());
    }
    
    // Helper method to get the path of the current file at compile time
    private static string GetFilePath([CallerFilePath] string filePath = "")
    {
        return filePath;
    }
}