using BEAM.Image.Minimap;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;

public partial class PlotMinimapConfigControlViewModel(PlotMinimap plotMinimap): ViewModelBase, ISaveControl
{
    [ObservableProperty] public int lineCompaction = plotMinimap.CompactionFactor;
    public void Save()
    {
        plotMinimap.CompactionFactor = LineCompaction;
    }
}