using BEAM.Docking;
using CommunityToolkit.Mvvm.ComponentModel;
using ScottPlot;

namespace BEAM.ViewModels.Minimap;

public partial class MinimapPlotViewModel : ViewModelBase, IDockBase
{
    [ObservableProperty] private Plot _currentPlot;
    public string Name { get; } = "Minimap View";
    public void OnClose()
    {
        return;
    }

    public MinimapPlotViewModel(Plot plot)
    {
        _currentPlot = plot;
        CurrentPlot = plot;
    }
}