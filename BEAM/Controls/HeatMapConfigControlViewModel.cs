// (c) Paul Stier, 2025

using BEAM.Renderer;
using BEAM.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.Controls;

/**
 * Viewmodel representing the configuration for <see cref="HeatMapRenderer"/>.
 * <param name="renderer">The renderer to configure</param>
 * <param name="model">The sequence view model where to get information about the loaded sequence from</param>
 */
public partial class HeatMapConfigControlViewModel(HeatMapRenderer renderer, SequenceViewModel model)
    : ViewModelBase, ISaveControl
{
    /**
     * The selected channel number.
     */
    [ObservableProperty] public partial decimal Channel { get; set; } = renderer.Channel;

    /**
     * The minimum channel number.
     */
    [ObservableProperty] public partial decimal Min { get; set; } = 0;

    /**
     * The maximum channel number.
     */
    [ObservableProperty] public partial decimal Max { get; set; } = model.Sequence.Shape.Channels - 1;

    public void Save()
    {
        renderer.Channel = (int)Channel;
    }
}