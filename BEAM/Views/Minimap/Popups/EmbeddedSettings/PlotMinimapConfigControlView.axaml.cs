using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BEAM.Image.Minimap;
using BEAM.ViewModels.Minimap.Popups;
using BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;

namespace BEAM.Views.Minimap.Popups.EmbeddedSettings;

public partial class PlotMinimapConfigControlView : UserControl, ISaveControl
{
    public PlotMinimapConfigControlView(PlotMinimap plotMinimap)
    {
        DataContext = new PlotMinimapConfigControlViewModel(plotMinimap);
        InitializeComponent();
    }
    
    public void Save()
    {
        ((DataContext as PlotMinimapConfigControlViewModel)!).Save();
    }
}