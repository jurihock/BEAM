using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using BEAM.ViewModels.AnalysisViewModels;
using ExCSS;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Plottables;

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
    }
    

    /// <summary>
    /// Displays the given parameters as a bar chart
    /// </summary>
    /// <param name="dataHeights"> The heights of all bars of the graph in the given order.</param>
    private void FillPlot(double[] dataHeights)
    {
        AvaPlot resultPlot = this.Find<AvaPlot>("AvaPlotAnalysis");
        resultPlot.Plot.Clear();
        var barPlot = resultPlot.Plot.Add.Bars(dataHeights);
        
        AddLabels(barPlot);
        
        resultPlot.Plot.Axes.Margins(bottom:0, top:.2f);
        resultPlot.Refresh();
    }

    private void AddLabels(BarPlot plot)
    {
        if (!_showLabels)
        {
            return;
        }
        foreach (var bar in plot.Bars)
        {
            bar.Label = bar.Value.ToString();
        }
        plot.ValueLabelStyle.Bold = true;
        plot.ValueLabelStyle.FontSize = 18;
    }

    public override void Update(double[] newData)
    {
        FillPlot(newData);
    }
}