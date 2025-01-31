using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using BEAM.ViewModels.AnalysisViewModels;
using ExCSS;
using ScottPlot;
using ScottPlot.Avalonia;

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
        AvaPlot resultPlot = this.Find<AvaPlot>("AvaPlotAnalysis");
        resultPlot.Plot.Clear();
        var barPlot = resultPlot.Plot.Add.Bars(dataHeights);
        
        if (_showLabels)
        {
            foreach (var bar in barPlot.Bars)
            {
                bar.Label = bar.Value.ToString();
            }
            barPlot.ValueLabelStyle.Bold = true;
            barPlot.ValueLabelStyle.FontSize = 18;
        }

        resultPlot.Plot.Axes.Margins(bottom:0, top:.2f);
        resultPlot.Refresh();
    }

    public override void Update(double[] newData)
    {
        FillPlot(newData);
    }
}