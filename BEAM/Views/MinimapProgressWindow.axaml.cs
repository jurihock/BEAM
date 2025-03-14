using Avalonia.Controls;
using BEAM.ViewModels.Minimap;

namespace BEAM.Views;

public partial class MinimapProgressWindow : Window
{
    public MinimapProgressWindow(MinimapPlotViewModel minimapPlotViewModel)
    {
        InitializeComponent();
        DataContext = minimapPlotViewModel;
    }

}