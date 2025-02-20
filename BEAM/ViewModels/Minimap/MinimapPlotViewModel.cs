using BEAM.Docking;
using BEAM.ViewModels.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using ScottPlot;

namespace BEAM.ViewModels.Minimap;

public partial class MinimapPlotViewModel : SizeAdjustableViewModelBase, IDockBase
{
    [ObservableProperty] private Plot _currentPlot;
    
    public string Name { get; } = "Minimap View";
    public void OnClose()
    {
    }

    public MinimapPlotViewModel(Plot plot)
    {
        _currentPlot = plot;
        CurrentPlot = plot;
    }

    public void Dispose()
    {
        CurrentPlot.Dispose();
    }
}
