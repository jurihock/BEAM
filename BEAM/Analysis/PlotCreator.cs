using System.Globalization;
using System;
using System.Linq;
using Avalonia;
using Avalonia.Styling;
using BEAM.Datatypes.Color;
using ScottPlot;
using ScottPlot.AxisRules;
using ScottPlot.Plottables;
using Colors = Avalonia.Media.Colors;

namespace BEAM.Analysis;

/// <summary>
/// Utility Class for creating basic, formatted Scottplot plots to display analysis Results.
/// </summary>
public static class PlotCreator
{
    private static void SetFigureBackgroundColor(Plot plot)
    {
        Avalonia.Media.Color backgroundColor;
        var currentTheme = Application.Current?.ActualThemeVariant ?? ThemeVariant.Default;

        try
        {
            Application.Current!.TryGetResource("WindowBg", currentTheme, out var background);
            backgroundColor = (Avalonia.Media.Color)background!;
        }
        catch (NullReferenceException)
        {
            backgroundColor = Colors.White;
        }
        
        plot.FigureBackground.Color = new Color(backgroundColor.R, backgroundColor.G, backgroundColor.B);
    }
    
    private static void SetBackgroundColor(Plot plot)
    {
        Avalonia.Media.Color backgroundColor;
        var currentTheme = Application.Current?.ActualThemeVariant ?? ThemeVariant.Default;

        try
        {
            Application.Current!.TryGetResource("BackgroundColorDark", currentTheme, out var background);
            backgroundColor = (Avalonia.Media.Color)background!;
        }
        catch (NullReferenceException)
        {
            backgroundColor = Colors.Gray;
        }

        plot.DataBackground.Color = new Color(backgroundColor.R, backgroundColor.G, backgroundColor.B);
    }

    private static void SetAxesAndFontColor(Plot plot)
    {
        Avalonia.Media.Color fontColor;
        var currentTheme = Application.Current?.ActualThemeVariant ?? ThemeVariant.Default;

        try
        {
            Application.Current!.TryGetResource("FontColor", currentTheme, out var fontColorScottPlot);
            fontColor = (Avalonia.Media.Color)fontColorScottPlot!;
        }
        catch (NullReferenceException)
        {
            fontColor = Colors.Black;
        }

        plot.Axes.Color(new Color(fontColor.R, fontColor.G, fontColor.B));
    }

    private static void SetGridLineColor(Plot plot)
    {
        Avalonia.Media.Color lineColor;
        var currentTheme = Application.Current?.ActualThemeVariant ?? ThemeVariant.Default;

        try
        {
            Application.Current!.TryGetResource("Separator", currentTheme, out var majorLines);
            lineColor = (Avalonia.Media.Color)majorLines!;
        }
        catch (NullReferenceException)
        {
            lineColor = Colors.LightGray;
        }

        plot.Grid.MajorLineColor = new Color(lineColor.R, lineColor.G, lineColor.B);
    }

    private static void SetForegroundColor(BarPlot barPlot)
    {
        Avalonia.Media.Color fontColor;
        var currentTheme = Application.Current?.ActualThemeVariant ?? ThemeVariant.Default;

        try
        {
            Application.Current!.TryGetResource("FontColor", currentTheme, out var fontColorScottPlot);
            fontColor = (Avalonia.Media.Color)fontColorScottPlot!;
        }catch (NullReferenceException)
        {
            fontColor = Colors.BlueViolet;
        }
        
        barPlot.ValueLabelStyle.ForeColor = new Color(fontColor.R, fontColor.G, fontColor.B);
    }
    
    private static void SetAccentColor(BarPlot barPlot)
    {
        Avalonia.Media.Color accentCol;
        var currentTheme = Application.Current?.ActualThemeVariant ?? ThemeVariant.Default;

        try
        {
            Application.Current!.TryGetResource("Accent", currentTheme, out var accentColor);
            accentCol = (Avalonia.Media.Color)accentColor!;
        }catch (NullReferenceException)
        {
            accentCol = Colors.Firebrick;
        }
        
        barPlot.Color = new Color(accentCol.R, accentCol.G, accentCol.B);
    }

    private static Avalonia.Media.Color GetAccentcolor()
    {
        var currentTheme = Application.Current?.ActualThemeVariant ?? ThemeVariant.Default;
        try
        {
            Application.Current!.TryGetResource("Accent", currentTheme, out var accentColor);
            return (Avalonia.Media.Color)accentColor!;
        }catch (NullReferenceException)
        {
            return Colors.Firebrick;
        }

    }

    private static void AddLabels(BarPlot barPlot)
    {
        foreach (var bar in barPlot.Bars)
        {
            bar.Label = Math.Round(bar.Value, 3).ToString(CultureInfo.InvariantCulture);
        }

        SetForegroundColor(barPlot);
        
        SetAccentColor(barPlot);

        barPlot.ValueLabelStyle.Bold = true;
        barPlot.ValueLabelStyle.FontSize = 18;
    }

    private static void SetLimitsAndLockMovement(Plot plot, double[] values)
    {
        var yAxisLimit = values.Max();
        if (yAxisLimit < 1)
        {
            yAxisLimit = 1;
        }

        double yAxisLimitBuffer = yAxisLimit * 0.15;
        plot.Axes.SetLimitsY(0, yAxisLimit + yAxisLimitBuffer);
        plot.Axes.SetLimitsX(-0.9, values.Length - 1 + 0.9);

        var limits = plot.Axes.GetLimits();
        var lockedVerticalRule = new LockedVertical(plot.Axes.Left, limits.Bottom, limits.Top);
        plot.Axes.Rules.Add(lockedVerticalRule);
        
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
    }

    private static void SetTickGeneratorIntegerAutoScaleX(Plot plot)
    {
        var bottomTickGen = new ScottPlot.TickGenerators.NumericAutomatic
        {
            IntegerTicksOnly = true
        };
        plot.Axes.Bottom.TickGenerator = bottomTickGen;
    }
    
    /// <summary>
    /// Creates a formatted bar plot, preventing scrolling to far in / out in regard to the given data.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static Plot CreateFormattedBarPlot(double[] values)
    {
        var plot = new Plot();
        
        // change figure colors
        SetFigureBackgroundColor(plot);

        SetBackgroundColor(plot);

        // change axis and grid colors
        SetAxesAndFontColor(plot);

        SetGridLineColor(plot);

        var barPlot = plot.Add.Bars(values);

        // Add labels
        if (values.Length <= 4)
        {
            AddLabels(barPlot);
        }

        SetLimitsAndLockMovement(plot, values);
        
        // Set X-Axis to have an autoscaling, integer tick step
        SetTickGeneratorIntegerAutoScaleX(plot);

        return plot;
    }

    /// <summary>
    /// This method will creat a placeholder plot that will be displayed when no sequence is selected.
    /// </summary>
    /// <returns></returns>
    public static Plot CreatePlaceholderPlot()
    {
        Plot plot = new();

        // change figure colors
        SetFigureBackgroundColor(plot);

        SetBackgroundColor(plot);

        // change axis and grid colors
        SetAxesAndFontColor(plot);

        SetGridLineColor(plot);

        Coordinates center = new(0, 0);
        double radiusX = 0.04;
        double radiusY = 0.2;
        var lineColor = GetAccentcolor();

        for (int i = 0; i < 5; i++)
        {
            float angle = (i * 20);
            var el = plot.Add.Ellipse(center, radiusX, radiusY, angle);
            el.LineWidth = 3;
            el.LineColor = new Color(barColor.R, barColor.G, barColor.B).WithAlpha(0.1 + 0.2 * i);
        }

        // Set limit of X- and Y-Axis
        plot.Axes.SetLimitsY(-5, 5);
        plot.Axes.SetLimitsX(-5, 5);

        // Lock movement of the 
        var limits = plot.Axes.GetLimits();
        var lockedVerticalRule = new LockedVertical(plot.Axes.Left, limits.Bottom, limits.Top);
        plot.Axes.Rules.Add(lockedVerticalRule);

        // Set X-Axis to have an autoscaling, integer tick step
        SetTickGeneratorIntegerAutoScaleX(plot);

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