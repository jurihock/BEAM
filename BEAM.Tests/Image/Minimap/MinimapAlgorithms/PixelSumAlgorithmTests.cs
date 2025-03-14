using System.Runtime.CompilerServices;
using BEAM.Exceptions;
using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.ImageSequence;
using BEAM.Renderer;

namespace BEAM.Tests.Image.Minimap.MinimapAlgorithms;

public class PixelSumAlgorithmTests
{
    [Fact]
    public void AnalyzeLine_ReturnsCorrectData()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../../../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var render = new ChannelMapRenderer(0, 255, 2, 1, 0);
        
        var analysis = new PixelSumAlgorithm();
        analysis.SetRenderer(render);
        analysis.AnalyzeSequence(sequence, CancellationToken.None);
        
        var result = analysis.GetLineValue(0);
        Assert.Equal(0, result);

        result = analysis.GetLineValue(439);
        Assert.Equal(80, Math.Floor(result));
    }
    
    [Fact]
    public void AnalyzeLine_ThrowsExceptionWhenEmpty()
    {
        var analysis = new PixelSumAlgorithm();
        Assert.Throws<InvalidStateException>(() => analysis.GetLineValue(0));
    }
    
    [Fact]
    public void AnalyzeLine_ThrowsExceptionWhenInvalidLine()
    {
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../../../TestAssets/Test.png") };

        var sequence = new SkiaSequence(list, "CoolSequence");
        var render = new ChannelMapRenderer(0, 255, 2, 1, 0);
        
        var analysis = new PixelSumAlgorithm();
        analysis.SetRenderer(render);
        analysis.AnalyzeSequence(sequence, CancellationToken.None);
        
        Assert.Throws<ArgumentOutOfRangeException>(() => analysis.GetLineValue(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => analysis.GetLineValue(900));
    }
    
    // Helper method to get the path of the current file at compile time
    private static string GetFilePath([CallerFilePath] string filePath = "")
    {
        return filePath;
    }
}