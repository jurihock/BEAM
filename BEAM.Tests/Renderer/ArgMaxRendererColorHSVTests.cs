using System.Runtime.CompilerServices;
using BEAM.Controls;
using BEAM.Datatypes.Color;
using BEAM.ImageSequence;
using BEAM.Renderer;
using Xunit.Abstractions;

namespace BEAM.Tests.Renderer;

public class ArgMaxRendererColorHSVTests
{

    [Fact]
    public void RenderPixel_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var renderer = new ArgMaxRendererColorHSV(0, 255);
        
        renderer.UpdateChannelHSVMap(new ChannelHSVMap(4).ToArray());
        var result = renderer.RenderPixel(sequence, 0, 0);
        Assert.Equal(new BGR(0, 0, 255), result);
        
        result = renderer.RenderPixel(sequence, 550, 120);
        Assert.Equal(new BGR(255, 0, 127), result);

        result = renderer.RenderPixel(sequence, 440, 169);
        Assert.Equal(new BGR(255, 255, 0), result);
    }

    [Fact]
    public void ArgMaxGrey_ThrowsOnUninitializedChannelLength()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var renderer = new ArgMaxRendererColorHSV(0, 255);
        
        Assert.Throws<ArgumentException>(() =>renderer.RenderPixel(sequence, 0, 0));
    }
    
    [Fact]
    public void ArgMaxGrey_ReturnsCorrectName()
    {
        Assert.Equal("ArgMax (Color HSV)", new ArgMaxRendererColorHSV(0, 255).GetName());
    }
    
    // Helper method to get the path of the current file at compile time
    private static string GetFilePath([CallerFilePath] string filePath = "")
    {
        return filePath;
    }
}