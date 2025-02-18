using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BEAM.Image.Minimap;
using BEAM.ViewModels.Minimap.Popups;
using BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;

namespace BEAM.Views.Minimap.Popups.EmbeddedSettings;

public partial class PlotMinimapConfigControlView : ISaveControl
{
    public PlotMinimapConfigControlView(PlotMinimap plotMinimap)
    {
        DataContext = new PlotMinimapConfigControlViewModel(plotMinimap);
        InitializeComponent();
    }
    
    public PlotMinimapConfigControlView()
    {
        InitializeComponent();
    }
    
    public override void Save()
    {
        ((DataContext as PlotMinimapConfigControlViewModel)!).Save();
    }
    

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