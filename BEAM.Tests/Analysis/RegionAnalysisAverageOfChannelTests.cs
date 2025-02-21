using System.Runtime.CompilerServices;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.Analysis;
using ScottPlot;
using Xunit;
using Moq;
using NP.Utilities;
using ScottPlot.Plottables;
using Xunit.Abstractions;

public class RegionAnalysisAverageOfChannelsTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public RegionAnalysisAverageOfChannelsTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void RegionAnalysisAverageOfChannels_Analyze_CalculatesCorrectResults()
    {
        var analysis = new RegionAnalysisAverageOfChannels();
        var path = GetFilePath().Split(Path.DirectorySeparatorChar).SkipLast(1);
        var list = new List<string> { Path.Combine(string.Join(Path.DirectorySeparatorChar, path), "../TestAssets/Test.png") };
        var sequence = new SkiaSequence(list, "Test.png");
        var start = new Coordinate2D(0, 0);
        var end = new Coordinate2D(100, 100);
        var result = analysis.Analyze(start, end, sequence);
        Assert.NotNull(result);

        var bars = result.GetPlottables<BarPlot>().FirstOrDefault();
        Assert.NotNull(result);
        Assert.Equal(0, bars.Bars[0].Value);
        Assert.Equal(0, bars.Bars[1].Value);
        Assert.Equal(0, bars.Bars[2].Value);
        Assert.Equal(0, bars.Bars[3].Value);
        
        var newStart = new Coordinate2D(300, 400);
        var newEnd = new Coordinate2D(400, 500);
        var newResult = analysis.Analyze(newStart, newEnd, sequence);
        Assert.NotNull(newResult);
        
        var newBars = newResult.GetPlottables<BarPlot>().FirstOrDefault();
        Assert.NotNull(newBars);
        Assert.Equal(89, Math.Round(newBars.Bars[0].Value));
        Assert.Equal(92, Math.Round(newBars.Bars[1].Value));
        Assert.Equal(181, Math.Round(newBars.Bars[2].Value));
        Assert.Equal(234, Math.Round(newBars.Bars[3].Value));
    }
    [Fact]
    public void ToString_ReturnsCorrectName()
    {
        var analysis = new RegionAnalysisAverageOfChannels();
        var result = analysis.ToString();

        Assert.Equal("Region Analysis Average", result);
    }
    
    // Helper method to get the path of the current file at compile time
    private static string GetFilePath([CallerFilePath] string filePath = "")
    {
        return filePath;
    }
}