// (c) Paul Stier, 2025

using BEAM.Renderer;
using BEAM.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Controls;

///View model for managing <see cref="ChannelMapRenderer"/> settings.
///<param name="renderer">The renderer to configure</param>
///<param name="model">The sequence view model where to get information about the loaded sequence from</param>
public partial class ChannelMapConfigControlViewModel(ChannelMapRenderer renderer, SequenceViewModel model)
    : ViewModelBase, ISaveControl
{
    /**
     * The red channel number.
     */
    [ObservableProperty]
    public partial decimal ChannelRed { get; set; } = renderer.ChannelRed;

    /**
    * The green channel number.
     */
    [ObservableProperty]
    public partial decimal ChannelGreen { get; set; } = renderer.ChannelGreen;

    /**
     * The blue channel number.
     */
    [ObservableProperty]
    public partial decimal ChannelBlue { get; set; } = renderer.ChannelBlue;

    /**
     * The minimum channel number.
     */
    [ObservableProperty]
    public partial decimal MinChannel { get; set; } = 0;

    /**
     * The maximum channel number.
     */
    [ObservableProperty]
    public partial decimal MaxChannel { get; set; } = model.Sequence.Shape.Channels - 1;

    public void Save()
    {
        renderer.ChannelRed = (int)ChannelRed;
        renderer.ChannelGreen = (int)ChannelGreen;
        renderer.ChannelBlue = (int)ChannelBlue;
    }
}