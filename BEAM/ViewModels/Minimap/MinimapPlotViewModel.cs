using BEAM.Docking;
using CommunityToolkit.Mvvm.ComponentModel;
using ScottPlot;

namespace BEAM.ViewModels.Minimap;

public partial class MinimapPlotViewModel : ViewModelBase, IDockBase
{
    [ObservableProperty] private Plot _currentPlot;
    public string Name { get; } = "Minimap View";

    public MinimapPlotViewModel(Plot plot)
    {
        _currentPlot = plot;
    }
}