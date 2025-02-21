using BEAM.Image.Minimap.MinimapAlgorithms;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;

/// <summary>
/// View model for configuring channel-specific threshold settings in minimap rendering.
/// Manages threshold values and channel selection for the rendered channel threshold algorithm.
/// </summary>
public partial class RenderedChannelThresholdViewModel : ViewModelBase
{
    /// <summary>
    /// The algorithm instance being configured.
    /// </summary>
    private readonly RenderedChannelThresholdAlgorithm _algorithm;
    /// <summary>
    /// Gets or sets the threshold value for the selected channel.
    /// Values range from 0 to 255.
    /// </summary>
    [ObservableProperty] public partial byte SelectedThreshold { get; set; }
    /// <summary>
    /// Gets or sets the channel index to apply the threshold to.
    /// Valid values are: 0 (Blue), 1 (Green), 2 (Red).
    /// </summary>
    [ObservableProperty] public partial int Channel { get; set; }

    /// <summary>
    /// Initializes a new instance of the RenderedChannelThresholdViewModel with the specified algorithm.
    /// Loads initial threshold and channel values from the algorithm.
    /// </summary>
    /// <param name="algorithm">The algorithm instance to configure.</param>
    public RenderedChannelThresholdViewModel(RenderedChannelThresholdAlgorithm algorithm)
    {
        _algorithm = algorithm;
        SelectedThreshold = algorithm.ChannelThreshold;
        Channel = algorithm.Channel;
    }

    /// <summary>
    /// Saves the current threshold and channel settings back to the algorithm.
    /// </summary>
    public void Save()
    {
        _algorithm.ChannelThreshold = SelectedThreshold;
        _algorithm.Channel = Channel;
    }
}