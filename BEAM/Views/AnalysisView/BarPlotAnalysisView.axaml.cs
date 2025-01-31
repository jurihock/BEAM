using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using BEAM.ViewModels.AnalysisViewModels;
using ExCSS;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.AxisRules;

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
        
        if (_showLabels)
        {
            foreach (var bar in barPlot.Bars)
            {
                bar.Label = bar.Value.ToString();
            }
            barPlot.ValueLabelStyle.Bold = true;
            barPlot.ValueLabelStyle.FontSize = 18;
        }
        
        AvaPlotAnalysis.Plot.Axes.SetLimitsX(-1, dataHeights.Length);
        AvaPlotAnalysis.Plot.Axes.SetLimitsY(0, dataHeights.Max() + 40);
        
        var limits = AvaPlotAnalysis.Plot.Axes.GetLimits(); 
        var lockedVerticalRule = new LockedVertical(AvaPlotAnalysis.Plot.Axes.Left, limits.Bottom, limits.Top);
        var lockedHorizontalRule = new LockedHorizontal(AvaPlotAnalysis.Plot.Axes.Bottom, limits.Left, limits.Right);
        
        AvaPlotAnalysis.Plot.Axes.Rules.Add(lockedVerticalRule);
        AvaPlotAnalysis.Plot.Axes.Rules.Add(lockedHorizontalRule);
        
        AvaPlotAnalysis.Plot.Axes.Margins(bottom:0, top:.2f);
        AvaPlotAnalysis.Refresh();
    }

    public override void Update(Rectangle rectangle, Sequence sequence)
    {
        var pixelData = sequence.GetPixel((long) rectangle.BottomRight.Column, (long) rectangle.BottomRight.Row);        
        FillPlot(pixelData);
        Console.WriteLine("test");
    }
}