using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BEAM.Image.Minimap;
using BEAM.ViewModels.Minimap.Popups;
using BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;

namespace BEAM.Views.Minimap.Popups.EmbeddedSettings;

public partial class StoredPlotMinimapConfigControlView : ISaveControl
{
    public StoredPlotMinimapConfigControlView(PlotMinimap plot, SettingsStorer storer)
    {
        DataContext = new StoredPlotMinimapConfigControlViewModel(plot, storer);
        InitializeComponent();
    }
    
    

    
    public override void Save()
    {
        ((DataContext as StoredPlotMinimapConfigControlViewModel)!).Save();
    }
    

    private void AlgorithmSelector_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var vm = (DataContext as StoredPlotMinimapConfigControlViewModel);
        if (vm is null)
        {
            return;
        }
        vm.SelectionChanged(sender, e);
    }
}