using BEAM.Image.Minimap.MinimapAlgorithms;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;

/// <summary>
/// View model for configuring pixel threshold settings for the <see cref="RenderedPixelAnyThresholdAlgorithm"/>.
/// </summary>
public partial class PixelThresholdAnySumAlgorithmConfigControlViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the threshold value for the red channel.
    /// </summary>
    [ObservableProperty] public partial byte SelectedRedThreshold { get; set; }
    /// <summary>
    /// Gets or sets the threshold value for the green channel.
    /// </summary>
    [ObservableProperty] public partial byte SelectedGreenThreshold { get; set; }
    /// <summary>
    /// Gets or sets the threshold value for the blue channel.
    /// </summary>
    [ObservableProperty] public partial byte SelectedBlueThreshold { get; set; }
    /// <summary>
    /// Gets or sets the threshold value for the alpha channel.
    /// </summary>
    [ObservableProperty] public partial byte SelectedAlphaThreshold { get; set; }

    private readonly RenderedPixelAnyThresholdAlgorithm _algorithm;
    /// <summary>
    /// Initializes a new instance with the specified algorithm and loads its current threshold values.
    /// </summary>
    /// <param name="algorithm">The algorithm instance to configure.</param>
    public PixelThresholdAnySumAlgorithmConfigControlViewModel(RenderedPixelAnyThresholdAlgorithm algorithm)
    {
        _algorithm = algorithm;
        SelectedRedThreshold = algorithm.ThresholdRed;
        SelectedGreenThreshold = algorithm.ThresholdGreen;
        SelectedBlueThreshold = algorithm.ThresholdBlue;
        SelectedAlphaThreshold = algorithm.ThresholdAlpha;
    }

    /// <summary>
    /// Saves the current threshold values back to the algorithm.
    /// </summary>
    public void Save()
    {
        _algorithm.ThresholdRed = SelectedRedThreshold;
        _algorithm.ThresholdGreen = SelectedGreenThreshold;
        _algorithm.ThresholdBlue = SelectedBlueThreshold;
        _algorithm.ThresholdAlpha = SelectedAlphaThreshold;
    }
}