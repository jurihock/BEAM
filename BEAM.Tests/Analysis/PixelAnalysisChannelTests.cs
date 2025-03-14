using Avalonia.Headless.XUnit;
using BEAM.Analysis;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using Moq;
using ScottPlot.Plottables;
using Xunit;

namespace BEAM.Tests.Analysis;

[Collection("GlobalTests")]
public class PixelAnalysisChannelTests
{
    
    [AvaloniaFact]
    public void AnalyzeForPlot_ReturnsPlotWithCorrectData_WhenCalledWithValidCoordinates()
    {
        var pointerPressedPoint = new Coordinate2D(0, 0);
        var pointerReleasedPoint = new Coordinate2D(1, 1);
        var sequenceMock = new Mock<ISequence>();
        sequenceMock.Setup(s => s.GetPixel(It.IsAny<long>(), It.IsAny<long>())).Returns(new double[] { 1, 2, 3 });

        var analysis = new PixelAnalysisChannel();
        var plot = analysis.AnalyzeForPlot(pointerPressedPoint, pointerReleasedPoint, sequenceMock.Object);

        var barPlot = plot.GetPlottables().OfType<BarPlot>().FirstOrDefault();
        Assert.NotNull(barPlot);

        for (var i = 0; i < 3; i++)
        {
            Assert.Equal(i + 1, barPlot.Bars[i].Value);
        }
    }

    [Fact]
    public void Analyze_ReturnsEmptyPlot_WhenSequenceReturnsEmptyArray()
    {
        var pointerPressedPoint = new Coordinate2D(0, 0);
        var pointerReleasedPoint = new Coordinate2D(1, 1);
        var sequenceMock = new Mock<ISequence>();
        sequenceMock.Setup(s => s.GetPixel(It.IsAny<long>(), It.IsAny<long>())).Returns(new double[] { });
        
        var analysis = new PixelAnalysisChannel();
        Assert.Throws<InvalidOperationException>(() => analysis.AnalyzeForPlot(pointerPressedPoint, pointerReleasedPoint, sequenceMock.Object));
    }

    [Fact]
    public void ToString_ReturnsCorrectName()
    {
        var analysis = new PixelAnalysisChannel();
        var result = analysis.ToString();

        Assert.Equal("Pixel Channel Analysis", result);
    }
}