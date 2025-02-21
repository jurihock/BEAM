using Avalonia.Controls;
using BEAM.Image.Minimap;
using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;
using BEAM.Views.Utility;

namespace BEAM.Views.Minimap.Popups.EmbeddedSettings;

/// <summary>
/// Represents a user control for configuring <see cref="PlotMinimap"/>  settings and <see cref="IMinimapAlgorithm"/>.
/// </summary>
public partial class PlotMinimapConfigControlView : SaveUserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlotMinimapConfigControlView"/> class with a specified plot minimap.
    /// </summary>
    /// <param name="plotMinimap">The plot minimap to configure.</param>
    public PlotMinimapConfigControlView(PlotMinimap plotMinimap)
    {
        DataContext = new PlotMinimapConfigControlViewModel(plotMinimap);
        InitializeComponent();
    }
    
    /// <summary>
    /// Saves the current configuration settings through the view model.
    /// </summary>
    public override void Save()
    {
        ((DataContext as PlotMinimapConfigControlViewModel)!).Save();
    }
    

    /// <summary>
    /// Handles algorithm selection changes in the UI.
    /// </summary>
    /// <param name="sender">The source of the selection change event.</param>
    /// <param name="e">Event data containing the selected algorithm.</param>
    private void AlgorithmSelector_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var vm = (DataContext as PlotMinimapConfigControlViewModel);
        if (vm is null)
        {
            return;
        }
        vm.SelectionChanged(sender, e);
    }
}