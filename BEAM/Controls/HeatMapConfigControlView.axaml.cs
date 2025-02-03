using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BEAM.Renderer;

namespace BEAM.Controls;

public partial class HeatMapConfigControlView : UserControl
{
    public HeatMapConfigControlView(HeatMapRenderer renderer)
    {
        DataContext = new HeatMapConfigControlViewModel(renderer);
        InitializeComponent();
    }
}