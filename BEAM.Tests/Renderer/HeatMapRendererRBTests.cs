using System.Runtime.CompilerServices;
using BEAM.Datatypes.Color;
using BEAM.ImageSequence;
using BEAM.Renderer;
using Xunit.Abstractions;

namespace BEAM.Tests.Renderer;

public class HeatMapRendererRBTests
{

    [Fact]
    public void RenderPixel_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var renderer = new HeatMapRendererRB(0, 255, 0, 0.1, 0.9);
        
        var result = renderer.RenderPixel(sequence, 436, 256);
        Assert.Equal(new BGR(6, 0, 249), result);
        
        result = renderer.RenderPixel(sequence, 419, 238);
        Assert.Equal(new BGR(219, 0, 36), result);
    }

    [Fact]
    public void RenderPixelSecondChannel_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var renderer = new HeatMapRendererRB(0, 255, 1, 0.1, 0.9);
        
        var result = renderer.RenderPixel(sequence, 436, 256);
        Assert.Equal(new BGR(6, 0, 249), result);
        
        result = renderer.RenderPixel(sequence, 419, 238);
        Assert.Equal(new BGR(219, 0, 36), result);
    }
    
    [Fact]
    public void RenderPixelThirdChannel_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var renderer = new HeatMapRendererRB(0, 255, 2, 0.1, 0.9);
        
        var result = renderer.RenderPixel(sequence, 436, 256);
        Assert.Equal(new BGR(6, 0, 249), result);
        
        result = renderer.RenderPixel(sequence, 419, 238);
        Assert.Equal(new BGR(87, 0, 168), result);
    }
    
    [Fact]
    public void RenderPixelFourthChannel_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var renderer = new HeatMapRendererRB(0, 255, 3, 0.1, 0.9);
        
        var result = renderer.RenderPixel(sequence, 436, 256);
        Assert.Equal(new BGR(0, 0, 255), result);
        
        result = renderer.RenderPixel(sequence, 419, 238);
        Assert.Equal(new BGR(31, 0, 224), result);
    }

    [Fact]
    public void GetPixels_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var renderer = new HeatMapRendererRB(0, 255, 3, 0.1, 0.9);
        BGR[] output = [new BGR(0, 0, 0), new BGR(0, 0, 0)];
        renderer.RenderPixels(sequence, [0, 419], 238, output);
        
        Assert.Equal(new BGR(255, 0, 0), output[0]);
        Assert.Equal(new BGR(31, 0, 224), output[1]);
    }
    
    [Fact]
    public void GetName_ReturnsCorrectName()
    {
        Assert.Equal("Heatmap", new HeatMapRendererRB(0, 255, 2, 0.1, 0.9).GetName());
    } 
    
    // Helper method to get the path of the current file at compile time
    private static string GetFilePath([CallerFilePath] string filePath = "")
    {
        return filePath;
    }
}