using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BEAM.Renderer;
using BEAM.ViewModels;

namespace BEAM.Controls;

/**
 * The view class for managing the settings of a <see cref="ChannelMapRenderer"/>.
 */
public partial class ChannelMapConfigControlView : UserControl, ISaveControl
{
    public ChannelMapConfigControlView(ChannelMapRenderer renderer, SequenceViewModel model)
    {
        DataContext = new ChannelMapConfigControlViewModel(renderer, model);
        InitializeComponent();
    }

    public void Save()
    {
        ((DataContext as ChannelMapConfigControlViewModel)!).Save();
    }
}