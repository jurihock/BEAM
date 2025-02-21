
using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;
using BEAM.Views.Utility;

namespace BEAM.Views.Minimap.Popups.EmbeddedSettings;

/// <summary>
/// Control view for configuring the pixel
/// threshold algorithm that sums up all pixels whose channel values are all larger than or equal to a baseline.
/// </summary>
public partial class PixelThresholdAllSumAlgorithmConfigControlView : SaveUserControl
{
    /// <summary>
    /// Initializes a new instance of the configuration view with the specified algorithm.
    /// </summary>
    /// <param name="algorithm">The algorithm instance to be configured.</param>
    public PixelThresholdAllSumAlgorithmConfigControlView(RenderedPixelAllThresholdAlgorithm algorithm)
    {
        InitializeComponent();
        DataContext = new PixelThresholdAllSumAlgorithmConfigControlViewModel(algorithm);
    }

    /// <summary>
    /// Saves the current configuration settings through the view model.
    /// </summary>
    public override void Save()
    {
        (DataContext as PixelThresholdAllSumAlgorithmConfigControlViewModel)!.Save();
    }
}