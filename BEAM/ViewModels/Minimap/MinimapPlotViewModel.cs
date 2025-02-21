using System;
using BEAM.Docking;
using BEAM.ViewModels.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using ScottPlot;

namespace BEAM.ViewModels.Minimap;

/// <summary>
/// View model for managing minimap plot display and interactions.
/// </summary>
public partial class MinimapPlotViewModel : SizeAdjustableViewModelBase, IDockBase
{
    /// <summary>
    /// Gets or sets the current plot being displayed.
    /// </summary>
    [ObservableProperty] private Plot _currentPlot;


    /// <summary>
    /// Gets the display name of the minimap view.
    /// </summary>
    public string Name { get; } = "Minimap View";

    public void OnClose()
    {
    }

    /// <summary>
    /// Initializes a new instance with the specified plot.
    /// </summary>
    /// <param name="plot">The plot to display.</param>
    public MinimapPlotViewModel(Plot plot)
    {
        _currentPlot = plot;
        CurrentPlot = plot;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        CurrentPlot.Dispose();
    }
}
