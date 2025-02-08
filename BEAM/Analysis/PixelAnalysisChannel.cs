using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels;
using BEAM.Views.AnalysisView;
using ScottPlot;
using ScottPlot.AxisRules;
using ScottPlot.Plottables;

namespace BEAM.Analysis;

public class PixelAnalysisChannel : Analysis
{
    public override Plot Analyze(Coordinate2D pointerPressedPoint, Coordinate2D pointerReleasedPoint, Sequence sequence)
    {
        double[] channels = sequence.GetPixel((long)pointerPressedPoint.Column, (long)pointerReleasedPoint.Row);

        Plot plot = createPlot(channels);
        plot.Title("Pixel Channel Analysis");
        return plot;
    }

    public override string ToString()
    {
        return "Pixel Channel Analysis";

    }

    /// <summary>
    /// Create a new formatted bar plot
    /// </summary>
    /// <param name="channels"></param>
    /// <returns></returns>
    private Plot createPlot(double[] channels)
    {
        Plot plot = new Plot();
        var barPlot = plot.Add.Bars(channels);
        
        // Add labels
        if (channels.Length <= 4)
        {
            foreach (var bar in barPlot.Bars)
            {
                bar.Label = bar.Value.ToString();
            }
            barPlot.ValueLabelStyle.Bold = true;
            barPlot.ValueLabelStyle.FontSize = 18;
        }
        
        // Set limit of X- and Y-Axis
        var yAxisLimit = channels.Max();
        if (yAxisLimit < 1)
        {
            yAxisLimit = 1;
        }
        double yAxisLimitBuffer = yAxisLimit * 0.15;
        plot.Axes.SetLimitsY(0, yAxisLimit + yAxisLimitBuffer);
        plot.Axes.SetLimitsX(-0.9, channels.Length - 1 + 0.9);
        
        // Lock movement of the 
        var limits = plot.Axes.GetLimits();
        var lockedVerticalRule = new LockedVertical(plot.Axes.Left, limits.Bottom, limits.Top);
        plot.Axes.Rules.Add(lockedVerticalRule);
        
        // Set X-Axis to have an autoscaling, integer tick step
        var bottomTickGen = new ScottPlot.TickGenerators.NumericAutomatic
        {
            IntegerTicksOnly = true
        };
        plot.Axes.Bottom.TickGenerator = bottomTickGen;

        // Define boundary, user can not move outside (data always visible)
        // Prevents scrolling and panning too far out / away from data
        var maximumBoundary = new MaximumBoundary(plot.Axes.Bottom, plot.Axes.Left, limits);
        plot.Axes.Rules.Add(maximumBoundary);
       
        // Limits zooming and scrolling potential to reasonable values, depending on the amount of bars
        var minSpanRule = new MinimumSpan(plot.Axes.Bottom,
            plot.Axes.Left, 4, 0.5 * (yAxisLimit + yAxisLimitBuffer));
        
        var maxSpanRule = new MaximumSpan(plot.Axes.Bottom,
            plot.Axes.Left, channels.Length + 2, yAxisLimit + yAxisLimitBuffer);
        
        plot.Axes.Rules.Add(minSpanRule);
        plot.Axes.Rules.Add(maxSpanRule);

        return plot;
    }
}