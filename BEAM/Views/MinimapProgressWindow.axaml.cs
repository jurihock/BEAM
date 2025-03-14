using Avalonia.Controls;
using BEAM.ViewModels.Minimap;

namespace BEAM.Views;

/// <summary>
/// The popup window for showing the minimap's generation process.
/// </summary>
public partial class MinimapProgressWindow : Window
{
    public MinimapProgressWindow(MinimapPlotViewModel minimapPlotViewModel)
    {
        InitializeComponent();
        DataContext = minimapPlotViewModel;
    }

}