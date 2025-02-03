// (c) Paul Stier, 2025

using BEAM.Renderer;
using BEAM.ViewModels;

namespace BEAM.Controls;

public class ChannelMapConfigControlViewModel : ViewModelBase
{
    public ChannelMapRenderer Renderer {get; set;}

    public ChannelMapConfigControlViewModel(ChannelMapRenderer renderer)
    {
        Renderer = renderer;
    }
}