using Avalonia.Controls.Models.TreeDataGrid;
using BEAM.Renderer;
using BEAM.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Controls;

public partial class ArgMaxConfigControlViewModel(ArgMaxRendererColorHSV renderer, SequenceViewModel model)
    : ViewModelBase, ISaveControl
{
    [ObservableProperty] private partial ChannelHSVMap ChannelHsvMap { get; set; } = renderer.getChannelHsvMap();
    
    public FlatTreeDataGridSource
    
    public void Save()
    {
        renderer.UpdateChannelHSVMap(ChannelHsvMap);
        ChannelHsvMap = renderer.Clone();
    }
}