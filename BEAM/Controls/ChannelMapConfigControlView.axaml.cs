using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BEAM.Renderer;

namespace BEAM.Controls;

public partial class ChannelMapConfigControlView : UserControl
{
    public ChannelMapConfigControlView(ChannelMapRenderer renderer)
    {
        DataContext = new ChannelMapConfigControlViewModel(renderer);
        InitializeComponent();
    }
}