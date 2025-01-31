using System;
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

namespace BEAM.Views.AnalysisView;

/// <summary>
/// Control to display the result of an analysis as a bar chart.
/// </summary>
public partial class CoordinateAnalysisView : AbstractAnalysisView
{
    public bool ShowLabels { get; set; } = true;
    private bool _showLabels = true;
    public CoordinateAnalysisView()
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

    public override void Update(Rectangle rectangle, Sequence sequence)
    {
        startX.Text = rectangle.TopLeft.Column.ToString();
        startY.Text = rectangle.TopLeft.Row.ToString();
        
        endX.Text = rectangle.BottomRight.Column.ToString();
        endY.Text = rectangle.BottomRight.Row.ToString();
    }
}