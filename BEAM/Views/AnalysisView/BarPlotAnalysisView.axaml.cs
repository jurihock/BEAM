using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using BEAM.Analysis;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels.AnalysisViewModels;
using ExCSS;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.AxisRules;
using ScottPlot.Plottables;
using Rectangle = BEAM.Datatypes.Rectangle;

namespace BEAM.Views.AnalysisView;

/// <summary>
/// Control to display the result of an analysis as a bar chart.
/// </summary>
public partial class BarPlotAnalysisView : AbstractAnalysisView
{
    public bool ShowLabels { get; set; } = true;
    private bool _showLabels = true;

    public BarPlotAnalysisView()
    {
        InitializeComponent();

        // var limits = AvaPlotAnalysis.Plot.Axes.GetLimits();
        // var lockedVerticalRule = new LockedVertical(AvaPlotAnalysis.Plot.Axes.Left, limits.Bottom, limits.Top);
        // var lockedHorizontalRule = new LockedHorizontal(AvaPlotAnalysis.Plot.Axes.Bottom, limits.Left, limits.Right);
        //
        // AvaPlotAnalysis.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericFixedInterval(1);
        //
        // AvaPlotAnalysis.Plot.Axes.Rules.Add(lockedVerticalRule);
        // AvaPlotAnalysis.Plot.Axes.Rules.Add(lockedHorizontalRule);

        // AvaPlotAnalysis.Plot.HideAxesAndGrid();

        //DataContext = new AnalysisViewModelPlot();
        //default values for testing
        //FillPlot([1.4, 2.5, 3.4, 4.0, 50]);
    }


    /// <summary>
    /// Displays the given parameters as a bar chart
    /// </summary>
    /// <param name="dataHeights"> The heights of all bars of the graph in the given order.</param>
    private void FillPlot(double[] dataHeights)
    {
        AvaPlotAnalysis.Plot.Clear();


        var barPlot = AvaPlotAnalysis.Plot.Add.Bars(dataHeights);

        if (_showLabels && dataHeights.Length <= 4)
        {
            foreach (var bar in barPlot.Bars)
            {
                bar.Label = bar.Value.ToString();
            }
        
            barPlot.ValueLabelStyle.Bold = true;
            barPlot.ValueLabelStyle.FontSize = 18;
        }

        AvaPlotAnalysis.Plot.Benchmark.IsVisible = false;

        // AvaPlotAnalysis.Plot.Axes.SetLimitsX(-1, dataHeights.Length);

        var yAxisLimit = dataHeights.Max();
        if (yAxisLimit < 1)
        {
            yAxisLimit = 1;
        }

        double yAxisLimitBuffer = yAxisLimit * 0.15;
        
        AvaPlotAnalysis.Plot.Axes.SetLimitsY(0, yAxisLimit + yAxisLimitBuffer);
        
        AvaPlotAnalysis.Plot.Axes.SetLimitsX(-0.9, dataHeights.Length - 1 + 0.9);
        
        //
        var limits = AvaPlotAnalysis.Plot.Axes.GetLimits();
        var lockedVerticalRule = new LockedVertical(AvaPlotAnalysis.Plot.Axes.Left, limits.Bottom, limits.Top);
        
        // var lockedHorizontalRule = new LockedHorizontal(AvaPlotAnalysis.Plot.Axes.Bottom, limits.Left, limits.Right);
        //
        var bottomTickGen = new ScottPlot.TickGenerators.NumericAutomatic
        {
            IntegerTicksOnly = true
        };

        var maximumBoundary = new MaximumBoundary(AvaPlotAnalysis.Plot.Axes.Bottom, AvaPlotAnalysis.Plot.Axes.Left, limits);

        AvaPlotAnalysis.Plot.Axes.Rules.Add(maximumBoundary);
        
        AvaPlotAnalysis.Plot.Axes.Bottom.TickGenerator = bottomTickGen;
        
        //
        AvaPlotAnalysis.Plot.Axes.Rules.Add(lockedVerticalRule);
        // AvaPlotAnalysis.Plot.Axes.Rules.Add(lockedHorizontalRule);

        var minSpanRule = new MinimumSpan(AvaPlotAnalysis.Plot.Axes.Bottom,
            AvaPlotAnalysis.Plot.Axes.Left, 4, 0.5 * (yAxisLimit + yAxisLimitBuffer));
        
        var maxSpanRule = new MaximumSpan(AvaPlotAnalysis.Plot.Axes.Bottom,
            AvaPlotAnalysis.Plot.Axes.Left, dataHeights.Length + 2, yAxisLimit + yAxisLimitBuffer);
        
        AvaPlotAnalysis.Plot.Axes.Rules.Add(minSpanRule);
        AvaPlotAnalysis.Plot.Axes.Rules.Add(maxSpanRule);
        
        AvaPlotAnalysis.Plot.Axes.Margins(bottom: 0, top: .2f);
        AvaPlotAnalysis.Refresh();
    }


    public override void Update()
    {
        throw new NotImplementedException();
    }
}