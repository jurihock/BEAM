using System.Linq;
using BEAM.Analysis;
using ScottPlot;
using ScottPlot.Plottables;
using Xunit;

public class PlotCreatorTests
{
    [Fact]
    public void CreateFormattedBarPlot_ReturnsPlotWithCorrectLimits_WhenValuesAreProvided()
    {
        double[] values = { 1, 2, 3, 4 };
        var plot = PlotCreator.CreateFormattedBarPlot(values);

        var limits = plot.Axes.GetLimits();
        Assert.Equal(0, limits.Bottom);
        Assert.Equal(4.6, limits.Top, 1);
        Assert.Equal(-0.9, limits.Left, 1);
        Assert.Equal(3.9, limits.Right, 1);
    }

    [Fact]
    public void CreateFormattedBarPlot_SetsBarLabels_WhenValuesLengthIsLessThanOrEqualToFour()
    {
        double[] values = { 1, 2, 3, 4 };
        var plot = PlotCreator.CreateFormattedBarPlot(values);
        
        var barPlot = plot.GetPlottables().OfType<BarPlot>().FirstOrDefault();
        Assert.NotNull(barPlot);
        Assert.All(barPlot.Bars, bar => Assert.False(string.IsNullOrEmpty(bar.Label)));
    }

    [Fact]
    public void CreateFormattedBarPlot_DoesNotSetBarLabels_WhenValuesLengthIsGreaterThanFour()
    {
        double[] values = { 1, 2, 3, 4, 5 };
        var plot = PlotCreator.CreateFormattedBarPlot(values);

        var barPlot = plot.GetPlottables().OfType<BarPlot>().FirstOrDefault();
        Assert.NotNull(barPlot);
        Assert.All(barPlot.Bars, bar => Assert.True(string.IsNullOrEmpty(bar.Label)));
    }

    [Fact]
    public void CreatePlaceholderPlot_ReturnsPlotWithCorrectLimits()
    {
        var plot = PlotCreator.CreatePlaceholderPlot();

        var limits = plot.Axes.GetLimits();
        Assert.Equal(-5, limits.Bottom);
        Assert.Equal(5, limits.Top);
        Assert.Equal(-5, limits.Left);
        Assert.Equal(5, limits.Right);
    }

    [Fact]
    public void CreatePlaceholderPlot_AddsEllipsesToPlot()
    {
        var plot = PlotCreator.CreatePlaceholderPlot();

        var ellipses = plot.GetPlottables().OfType<Ellipse>().ToList();
        Assert.Equal(5, ellipses.Count);
    }
}