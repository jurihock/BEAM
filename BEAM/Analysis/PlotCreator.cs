using System.Globalization;
using System;
using System.Linq;
using ScottPlot;
using ScottPlot.AxisRules;

namespace BEAM.Analysis;

/// <summary>
/// Utility Class for creating basic, formatted Scottplot plots to display analysis Results.
/// </summary>
public static class PlotCreator
{
    /// <summary>
    /// Creates a formatted bar plot, preventing scrolling to far in / out in regard to the given data.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static Plot CreateFormattedBarPlot(double[] values)
    {
        Plot plot = new Plot();
        var barPlot = plot.Add.Bars(values);
        
        // Add labels
        if (values.Length <= 4)
        {
            foreach (var bar in barPlot.Bars)
            {
                bar.Label = Math.Round(bar.Value, 3).ToString(CultureInfo.InvariantCulture);
            }
            barPlot.ValueLabelStyle.Bold = true;
            barPlot.ValueLabelStyle.FontSize = 18;
        }
        
        // Set limit of X- and Y-Axis
        var yAxisLimit = values.Max();
        if (yAxisLimit < 1)
        {
            yAxisLimit = 1;
        }
        double yAxisLimitBuffer = yAxisLimit * 0.15;
        plot.Axes.SetLimitsY(0, yAxisLimit + yAxisLimitBuffer);
        plot.Axes.SetLimitsX(-0.9, values.Length - 1 + 0.9);
        
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
            plot.Axes.Left, values.Length + 2, yAxisLimit + yAxisLimitBuffer);
        
        plot.Axes.Rules.Add(minSpanRule);
        plot.Axes.Rules.Add(maxSpanRule);

        return plot;
    }
    
    /// <summary>
    /// This method will creat a placeholder plot that will be displayed when no sequence is selected.
    /// </summary>
    /// <returns></returns>
    public static Plot CreatePlaceholderPlot()
    {
        Plot plot = new();
        
        Coordinates center = new(0, 0);
        double radiusX = 1;
        double radiusY = 5;

        for (int i = 0; i < 5; i++)
        {
            float angle =(i * 20);
            var el = plot.Add.Ellipse(center, radiusX, radiusY, angle);
            el.LineWidth = 3;
            el.LineColor = Colors.Blue.WithAlpha(0.1 + 0.2 * i);
        }
        
        // Set limit of X- and Y-Axis
        plot.Axes.SetLimitsY(-5, 5);
        plot.Axes.SetLimitsX(-5, 5);
        
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

        plot.Layout.Frameless();
        plot.Axes.Margins(0, 0);
        plot.Title("No sequence selected");
        return plot;
    }
}