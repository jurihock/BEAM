using System.Runtime.CompilerServices;
using BEAM.Datatypes.Color;
using BEAM.ImageSequence;
using BEAM.Renderer;

namespace BEAM.Tests.Renderer;

public class ArgMaxRendererGreyTests
{
    [Fact]
    public void RenderPixel_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var renderer = new ArgMaxRendererGrey(0, 255);
        
        Assert.Equal(new BGR(0, 0, 0), renderer.RenderPixel(sequence, 0, 0));
        Assert.Equal(new BGR(191, 191, 191), renderer.RenderPixel(sequence, 448, 228));
        Assert.True(AllGrey(renderer, sequence));
    }

    [Fact]
    public void RenderPixels_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var renderer = new ArgMaxRendererGrey(0, 255);

        BGR[] output = [new BGR(0, 0, 0), new BGR(0, 0, 0)];
        var pixels = renderer.RenderPixels(sequence, [0, 448], 228, output);
        Assert.Equal(new BGR(0, 0, 0), pixels[0]);
        Assert.Equal(new BGR(191, 191, 191), pixels[1]);
    }

    [Fact]
    public void ArgMaxGrey_ReturnsCorrectName()
    {
        Assert.Equal("ArgMax (Gray Scale)", new ArgMaxRendererGrey(0, 255).GetName());
    }
    
    private static bool AllGrey(ArgMaxRendererGrey renderer, ISequence sequence)
    {
        for (var i = 0; i < sequence.Shape.Height; i++)
        {
            for (var j = 0; j < sequence.Shape.Width; j++)
            {
                var color = renderer.RenderPixel(sequence, j, i);
                if (color.R != color.G || color.G != color.B || color.B != color.R)
                {
                    return false;
                }
            }
        }

        return true;
    }
    
    // Helper method to get the path of the current file at compile time
    private static string GetFilePath([CallerFilePath] string filePath = "")
    {
        return filePath;
    }
}